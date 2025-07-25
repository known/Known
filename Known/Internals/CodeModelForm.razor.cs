﻿namespace Known.Internals;

/// <summary>
/// 代码生成表单组件类。
/// </summary>
public partial class CodeModelForm
{
    private readonly List<CodeInfo> Functions =
    [
        new("New", Language.New),
        new("Edit", Language.Edit),
        new("Delete", Language.Delete),
        new("DeleteM", Language.DeleteM),
        new("Import", Language.Import),
        new("Export", Language.Export)
    ];

    /// <summary>
    /// 取得或设置代码生成服务实例。
    /// </summary>
    [Parameter] public ICodeService Service { get; set; }

    /// <summary>
    /// 取得或设置代码生成模型默认信息。
    /// </summary>
    [Parameter] public CodeModelInfo Default { get; set; }

    /// <summary>
    /// 取得或设置代码生成模型信息。
    /// </summary>
    [Parameter] public CodeModelInfo Model { get; set; }

    /// <summary>
    /// 取得或设置代码生成模型保存后委托。
    /// </summary>
    [Parameter] public Action<CodeModelInfo> OnSaved { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Model ??= CreateCodeMode();
    }

    private void OnNew() => Model = CreateCodeMode();

    private void OnSelect()
    {
        if (UIConfig.OnSelectModel == null)
        {
            UI.Error(Language.TipNoConfigOnSelectModel);
            return;
        }

        UIConfig.OnSelectModel.Invoke(UI, model =>
        {
            Model = model;
            StateChanged();
        });
    }

    private async Task OnSave()
    {
        if (string.IsNullOrWhiteSpace(Model.Code) || string.IsNullOrWhiteSpace(Model.Name))
        {
            UI.Error(Language.TipCodeAndNameRequired);
            return;
        }

        var result = await Service.SaveModelAsync(Model);
        UI.Result(result, () =>
        {
            var info = result.DataAs<CodeModelInfo>();
            OnSaved?.Invoke(info);
            return Task.CompletedTask;
        });
    }

    //private void OnInsert() { }

    private void OnAdd() => Model.Fields.Add(new CodeFieldInfo());

    private void OnAddPlus()
    {
        if (UIConfig.OnFastAddField == null)
        {
            UI.Error(Language.TipNoConfigOnFastAddField);
            return;
        }

        var selectedItems = Model.Fields.Select(d => d.Id).ToList();
        UIConfig.OnFastAddField.Invoke(UI, selectedItems, items =>
        {
            var fields = items?.Select(CodeFieldInfo.FromField).ToList();
            if (fields != null && fields.Count > 0)
            {
                Model.Fields.AddRange(fields);
                StateChanged();
            }
        });
    }

    private async Task OnAddTable()
    {
        var tables = await Service.GetDbTablesAsync();
        var tableName = string.Empty;
        var model = new DialogModel
        {
            Title = Language.ImportTable,
            Content = b => b.Div().Style("padding:20px 30px 0 30px;").Child(() => b.Select(Language.DataTable, new InputModel<string>
            {
                Codes = tables,
                ValueChanged = this.Callback<string>(v => tableName = v)
            }))
        };
        model.OnOk = async () =>
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                UI.Error(Language.TipSelectDataTable);
                return;
            }

            Model.Fields = await Service.GetDbFieldsAsync(tableName);
            await model.CloseAsync();
        };
        UI.ShowDialog(model);
    }

    private Task OnPasteAsync()
    {
        return JSRuntime.PasteTextAsync(text =>
        {
            var lines = text.Split(Environment.NewLine.ToCharArray())
                            .Where(l => !string.IsNullOrWhiteSpace(l))
                            .ToList();
            Model.Fields.Clear();
            foreach (var line in lines)
            {
                var items = line.Contains('\t') ? line.Split('\t') : line.Split('|');
                var info = new CodeFieldInfo();
                if (items.Length > 0) info.Name = items[0].Trim();
                if (items.Length > 1) info.Id = items[1].Trim();
                if (items.Length > 2) info.Type = Utils.ConvertTo<FieldType>(items[2].Trim());
                if (items.Length > 3) info.Length = items[3].Trim();
                if (items.Length > 4) info.Required = Utils.ConvertTo<bool>(items[4].Trim());
                if (items.Length > 5) info.IsGrid = Utils.ConvertTo<bool>(items[5].Trim());
                if (items.Length > 6) info.IsForm = Utils.ConvertTo<bool>(items[6].Trim());
                Model.Fields.Add(info);
            }
        });
    }

    private void OnGridSetting(CodeFieldInfo row)
    {
        var form = new FormModel<PageColumnInfo>(this)
        {
            Title = Language.MoreTableSetting,
            Data = row.ToPageColumn(),
            Type = typeof(MoreGridSetting),
            OnSave = data => Result.SuccessAsync("")
        };
        UI.ShowForm(form);
    }

    private void OnFormSetting(CodeFieldInfo row)
    {
        var form = new FormModel<FormFieldInfo>(this)
        {
            Title = Language.MoreFormSetting,
            Data = row.ToFormField(),
            Type = typeof(MoreFormSetting),
            OnSave = data => Result.SuccessAsync("")
        };
        UI.ShowForm(form);
    }

    private void OnDelete(CodeFieldInfo row) => Model.Fields.Remove(row);
    private void OnMoveUp(CodeFieldInfo row) => MoveRow(row, true);
    private void OnMoveDown(CodeFieldInfo row) => MoveRow(row, false);

    private void MoveRow(CodeFieldInfo row, bool isMoveUp)
    {
        var index = Model.Fields.IndexOf(row);
        var index1 = isMoveUp ? index - 1 : index + 1;
        if (index1 < 0 || index1 > Model.Fields.Count - 1)
            return;

        var temp = Model.Fields[index1];
        Model.Fields[index1] = row;
        Model.Fields[index] = temp;
    }

    private CodeModelInfo CreateCodeMode()
    {
        var info = new CodeModelInfo { IsNew = true };
        if (Default != null)
        {
            info.Prefix = Default.Prefix;
            info.Namespace = Default.Namespace;
        }
        if (string.IsNullOrWhiteSpace(info.Namespace))
            info.Namespace = Config.App.Assembly.FullName.Split(',')[0].Replace(".Web", "");
        return info;
    }
}