namespace Known.Internals;

class AutoTablePage : BaseTablePage<Dictionary<string, object>>, IAutoPage
{
    private AutoDataService Service;
    private readonly Dictionary<string, object> defaultData = [];

    [Parameter] public string PageId { get; set; }

    public Task InitializeAsync()
    {
        InitTable();
        return base.RefreshAsync();
    }

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Service = await AutoDataService.CreateServiceAsync(this);
        await InitializeAsync();
    }

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        if (UIConfig.AutoTablePage != null)
            UIConfig.AutoTablePage.Invoke(builder, Table);
        else
            base.BuildPage(builder);
    }

    /// <summary>
    /// 弹出新增表单对话框。
    /// </summary>
    public void New() => Table.NewForm(Service.SaveModelAsync, defaultData);

    /// <summary>
    /// 批量删除多条数据。
    /// </summary>
    public void DeleteM() => Table.DeleteM(Service.DeleteModelsAsync);

    /// <summary>
    /// 弹出编辑表单对话框。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    public void Edit(Dictionary<string, object> row) => Table.EditForm(Service.SaveModelAsync, row);

    /// <summary>
    /// 删除一条数据。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    public void Delete(Dictionary<string, object> row) => Table.Delete(Service.DeleteModelsAsync, row);

    /// <summary>
    /// 弹出数据导入对话框。
    /// </summary>
    public Task Import() => Table.ShowImportAsync();

    /// <summary>
    /// 导出表格数据。
    /// </summary>
    public Task Export() => Table.ExportDataAsync();

    private void InitTable()
    {
        Table.Initialize(true);
        Table.OnQuery = Service.QueryModelsAsync;
        Table.Criteria.Clear();
        Table.SetQueryColumns();

        var param = Context?.Current?.GetAutoPageParameter();
        var model = DataHelper.ToEntity(param?.EntityData);
        var fields = model?.Fields;
        if (fields != null && fields.Count > 0)
        {
            foreach (var item in fields)
            {
                defaultData[item.Id] = null;
            }
        }
    }
}