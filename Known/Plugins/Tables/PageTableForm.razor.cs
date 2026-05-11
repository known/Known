namespace Known.Plugins.Tables;

/// <summary>
/// 页面配置弹窗表格组件类。
/// </summary>
public partial class PageTableForm
{
    //private List<FieldDataInfo> Fields = [];
    private TableModel<Dictionary<string, object>> Table;
    //private bool isPreview = false;

    private static string WrapClass => "kui-plugin-table";
    //private string WrapClass => $"kui-plugin-split{(isPreview ? " preview" : "")}";
    private string ToolName => $"工具栏({Model?.Page?.Tools?.Count})";
    private string ActionName => $"操作列({Model?.Page?.Actions?.Count})";
    private bool ShowAdd => Model?.PageType != AutoPageType.SqlQuery;
    private bool ShowPaste => Model?.PageType == AutoPageType.NewTable;

    /// <summary>
    /// 取得或设置无代码表格插件配置信息。
    /// </summary>
    [Parameter] public AutoPageInfo Model { get; set; }

    /// <summary>
    /// 取得或设置无代码表格插件配置信息改变委托。
    /// </summary>
    [Parameter] public Action ModelChanged { get; set; }

    internal override List<string> SelectedItems => Items?.Select(d => d.Id).ToList();

    /// <inheritdoc />
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        //var service = await CreateServiceAsync<IFieldService>();
        //Fields = await service.GetFieldsAsync();
        Items = Model.Page.Columns;

        Table = new DemoTableModel(this);
        PreviewPage();
    }

    internal override List<PageColumnInfo> GetSelectedRows(List<FieldDataInfo> rows)
    {
        return [.. rows.Select(r => r.ToColumn())];
    }

    private Task OnPasteAsync()
    {
        return JSRuntime.PasteTextAsync(text =>
        {
            var lines = text.Split(Environment.NewLine.ToCharArray())
                            .Where(l => !string.IsNullOrWhiteSpace(l))
                            .ToList();
            Items.Clear();
            foreach (var line in lines)
            {
                var items = line.Contains('\t') ? line.Split('\t') : line.Split('|');
                var info = new PageColumnInfo();
                if (items.Length > 0) info.Name = items[0].Trim();
                if (items.Length > 1) info.Id = items[1].Trim();
                if (items.Length > 2) info.Type = Utils.ConvertTo<FieldType>(items[2].Trim());
                if (items.Length > 3) info.Length = items[3].Trim();
                if (items.Length > 4) info.Required = Utils.ConvertTo<bool>(items[4].Trim());
                if (items.Length > 5) info.IsViewLink = Utils.ConvertTo<bool>(items[5].Trim());
                if (items.Length > 6) info.IsQuery = Utils.ConvertTo<bool>(items[6].Trim());
                if (items.Length > 7) info.IsSum = Utils.ConvertTo<bool>(items[7].Trim());
                if (items.Length > 8) info.IsSort = Utils.ConvertTo<bool>(items[8].Trim());
                if (items.Length > 9) info.DefaultSort = items[9].Trim();
                if (items.Length > 10) info.Fixed = items[10].Trim();
                if (items.Length > 11) info.Align = items[11].Trim();
                Items.Add(info);
            }
            OnRefresh?.Invoke();
        });
    }

    private void OnEditToolbar()
    {
        ToolTableForm form = null;
        var model = new DialogModel
        {
            Title = "工具栏设置",
            Width = 700,
            ClassName = "kui-plugin-form",
            Content = b => b.Component<ToolTableForm>().Set(c => c.Model, Model).Build(value => form = value)
        };
        model.OnOk = async () =>
        {
            Model.Page.Tools = form?.Items;
            PreviewPage();
            await model.CloseAsync();
        };
        UI.ShowDialog(model);
    }

    private void OnEditAction()
    {
        ActionTableForm form = null;
        var model = new DialogModel
        {
            Title = "操作列设置",
            Width = 700,
            ClassName = "kui-plugin-form",
            Content = b => b.Component<ActionTableForm>().Set(c => c.Model, Model).Build(value => form = value)
        };
        model.OnOk = async () =>
        {
            Model.Page.Actions = form?.Items;
            PreviewPage();
            await model.CloseAsync();
        };
        UI.ShowDialog(model);
    }

    private void OnPreview()
    {
        Table.Initialize(Model);
        var model = new DialogModel
        {
            Title = Language.Preview,
            Width = 1200,
            Maximizable = true,
            Content = b =>
            {
                b.Div("kui-plugin-page", () => b.Component<DemoTablePage>().Set(c => c.Model, Table).Build());
            }
        };
        UI.ShowDialog(model);
    }

    private void OnQuerySetting(PageColumnInfo row)
    {
        var form = new FormModel<PageColumnInfo>(this)
        {
            Title = "更多条件设置",
            Data = row,
            Type = typeof(MoreGridSetting),
            OnSave = data => Result.SuccessAsync("")
        };
        form.Parameters[nameof(MoreGridSetting.IsDefaultValue)] = true;
        UI.ShowForm(form);
    }

    private static void PreviewPage()
    {
        //Table.Initialize(Model);
        //ModelChanged?.Invoke();
    }

    private static void RefreshPage()
    {
        //PreviewPage();
        //StateChanged();
    }
}