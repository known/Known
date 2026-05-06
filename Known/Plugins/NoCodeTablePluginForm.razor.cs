namespace Known.Plugins;

/// <summary>
/// 单表无代码表格插件设置窗体。
/// </summary>
public partial class NoCodeTablePluginForm : BaseComponent
{
    private const string TabColumns = "Columns";
    private const string TabFields = "Fields";
    private const string TabScript = "Script";

    private string activeTab = TabColumns;

    [Inject] private ICodeGenerator Generator { get; set; }
    [Inject] private ICodeService CodeService { get; set; }

    /// <summary>
    /// 插件设置表单模型。
    /// </summary>
    [Parameter] public FormModel<AutoPageInfo> Model { get; set; }

    private string ScriptFile => $"{Model.Data.Script}.sql";

    private string CodeScript
    {
        get
        {
            var entity = BuildEntity();
            Generator.Model = BuildCodeModel();
            return Generator.GetScript(Config.DatabaseType, entity);
        }
    }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Normalize();
    }

    private void OnTabChanged(string tab)
    {
        activeTab = tab;
    }

    private void OnAddField()
    {
        var field = new CodeFieldInfo
        {
            Id = $"Field{Model.Data.Form.Fields.Count + 1}",
            Name = $"字段{Model.Data.Form.Fields.Count + 1}",
            Type = FieldType.Text
        };

        Model.Data.Page.Columns.Add(field.ToPageColumn());
        Model.Data.Form.Fields.Add(field.ToFormField());
        StateChanged();
    }

    private async Task OnImportTableAsync()
    {
        var tables = await CodeService.GetDbTablesAsync();
        var tableName = Model.Data.Script;
        var dialog = new DialogModel
        {
            Title = Language.ImportTable,
            Content = b => b.Div().Style("padding:20px 30px 0 30px;").Child(() => b.Select(Language.DataTable, new InputModel<string>
            {
                Value = tableName,
                Codes = tables,
                ValueChanged = this.Callback<string>(v => tableName = v)
            }))
        };
        dialog.OnOk = async () =>
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                UI.Error(Language.TipSelectDataTable);
                return;
            }

            var fields = await CodeService.GetDbFieldsAsync(tableName);
            ApplyFields(tableName, fields);
            await dialog.CloseAsync();
        };
        UI.ShowDialog(dialog);
    }

    private async Task OnExecuteAsync()
    {
        Normalize();
        if (string.IsNullOrWhiteSpace(Model.Data.Script))
        {
            UI.Error("数据表名为空，不能执行建表脚本！");
            return;
        }

        var info = new AutoInfo<string>
        {
            PageId = Model.Data.Id ?? Model.Data.Script,
            PluginId = CodeTab.Script,
            Data = CodeScript
        };
        var result = await CodeService.CreateTableAsync(info);
        UI.Result(result);
    }

    private void OnGridSetting(PageColumnInfo row)
    {
        var form = new FormModel<PageColumnInfo>(this)
        {
            Title = Language.MoreTableSetting,
            Data = row,
            Type = typeof(MoreGridSetting),
            OnSave = data => Result.SuccessAsync(string.Empty)
        };
        UI.ShowForm(form);
    }

    private void OnFormSetting(FormFieldInfo row)
    {
        var form = new FormModel<FormFieldInfo>(this)
        {
            Title = Language.MoreFormSetting,
            Data = row,
            Type = typeof(MoreFormSetting),
            OnSave = data => Result.SuccessAsync(string.Empty)
        };
        UI.ShowForm(form);
    }

    private void OnMoveUpColumn(PageColumnInfo row)
    {
        Model.Data.Page.Columns.MoveRow(row, true);
        StateChanged();
    }

    private void OnDeleteColumn(PageColumnInfo row)
    {
        Model.Data.Page.Columns.Remove(row);
        StateChanged();
    }

    private void OnMoveUpField(FormFieldInfo row)
    {
        Model.Data.Form.Fields.MoveRow(row, true);
        StateChanged();
    }

    private void OnDeleteField(FormFieldInfo row)
    {
        Model.Data.Form.Fields.Remove(row);
        StateChanged();
    }

    private void Normalize()
    {
        Model.Data.Page ??= new PageInfo();
        Model.Data.Form ??= new FormInfo();
        Model.Data.Page.Columns ??= [];
        Model.Data.Form.Fields ??= [];
        Model.Data.Page.Tools ??= [];
        Model.Data.Page.Actions ??= [];
        Model.Data.IdField ??= nameof(EntityBase.Id);
    }

    private void ApplyFields(string tableName, List<CodeFieldInfo> fields)
    {
        Model.Data.Script = tableName;
        Model.Data.Name ??= tableName;
        Model.Data.Page.Columns.Clear();
        Model.Data.Form.Fields.Clear();

        foreach (var item in fields)
        {
            var column = item.ToPageColumn();
            column.IsQuery = item.Type is FieldType.Text or FieldType.Integer or FieldType.Number or FieldType.DateTime or FieldType.Date;
            column.IsViewLink = Model.Data.Page.Columns.Count == 0;
            Model.Data.Page.Columns.Add(column);

            var field = item.ToFormField();
            field.Span = 12;
            Model.Data.Form.Fields.Add(field);
        }

        if (Model.Data.Page.Tools == null || Model.Data.Page.Tools.Count == 0)
        {
            Model.Data.Page.Tools =
            [
                new ActionInfo("New"),
                new ActionInfo("DeleteM"),
                new ActionInfo("Import"),
                new ActionInfo("Export")
            ];
        }

        if (Model.Data.Page.Actions == null || Model.Data.Page.Actions.Count == 0)
        {
            Model.Data.Page.Actions =
            [
                new ActionInfo("Edit"),
                new ActionInfo("Delete")
            ];
        }

        StateChanged();
    }

    private EntityInfo BuildEntity()
    {
        var info = new EntityInfo
        {
            Id = Model.Data.Id,
            Name = Model.Data.Name,
            PageUrl = Model.Data.PageUrl,
            TableName = Model.Data.Script
        };

        foreach (var item in Model.Data.Form.Fields)
        {
            var field = item.ToField();
            field.IsForm = true;
            field.IsGrid = Model.Data.Page.Columns.Exists(d => d.Id == field.Id);
            info.Fields.Add(field);
        }

        return info;
    }

    private CodeModelInfo BuildCodeModel()
    {
        var entity = BuildEntity();
        return new CodeModelInfo
        {
            Code = Model.Data.Id,
            Name = Model.Data.Name,
            PageUrl = Model.Data.PageUrl,
            Entity = entity,
            Page = Model.Data.Page,
            Form = Model.Data.Form
        };
    }
}