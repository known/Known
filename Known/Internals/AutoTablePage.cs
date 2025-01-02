namespace Known.Internals;

class AutoTablePage : BaseTablePage<Dictionary<string, object>>, IAutoPage
{
    private IAutoService Service;
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
        Service = await CreateServiceAsync<IAutoService>();
        await InitializeAsync();
    }

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        if (UIConfig.AutoTablePage != null)
            UIConfig.AutoTablePage.Invoke(builder, Table);
        else
            builder.Table(Table);
    }

    /// <summary>
    /// 弹出新增表单对话框。
    /// </summary>
    public void New() => Table.NewForm(SaveModelAsync, defaultData);

    /// <summary>
    /// 批量删除多条数据。
    /// </summary>
    public void DeleteM() => Table.DeleteM(DeleteModelsAsync);

    /// <summary>
    /// 弹出编辑表单对话框。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    public void Edit(Dictionary<string, object> row) => Table.EditForm(SaveModelAsync, row);

    /// <summary>
    /// 删除一条数据。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    public void Delete(Dictionary<string, object> row) => Table.Delete(DeleteModelsAsync, row);

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
        Table.Initialize();
        Table.OnQuery = OnQueryModelsAsync;
        Table.Criteria.Clear();
        Table.SetQueryColumns();

        var plugin = Context?.Current?.Plugins?.GetPlugin<EntityPluginInfo>();
        var model = DataHelper.ToEntity(plugin?.EntityData);
        var fields = model?.Fields;
        if (fields != null && fields.Count > 0)
        {
            foreach (var item in fields)
            {
                defaultData[item.Id] = null;
            }
        }
    }

    private Task<PagingResult<Dictionary<string, object>>> OnQueryModelsAsync(PagingCriteria criteria)
    {
        criteria.Parameters[nameof(PageId)] = PageId;
        return Service.QueryModelsAsync(criteria);
    }

    private Task<Result> DeleteModelsAsync(List<Dictionary<string, object>> models)
    {
        var info = new AutoInfo<List<Dictionary<string, object>>>
        {
            PageId = PageId,
            Data = models
        };
        return Service.DeleteModelsAsync(info);
    }

    private Task<Result> SaveModelAsync(UploadInfo<Dictionary<string, object>> info)
    {
        info.PageId = PageId;
        return Service.SaveModelAsync(info);
    }
}