namespace Known.Pages;

/// <summary>
/// 无代码表格页面组件类。
/// </summary>
[StreamRendering]
[Route("/page/{*PageRoute}")]
public class AutoTablePage : BaseTablePage<Dictionary<string, object>>
{
    private IAutoService Service;
    private string pageRoute;
    private readonly Dictionary<string, object> defaultData = [];
    private string PageId { get; set; }

    /// <summary>
    /// 取得或设置页面路由。
    /// </summary>
    [Parameter] public string PageRoute { get; set; }

    /// <summary>
    /// 异步初始化页面。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Service = await CreateServiceAsync<IAutoService>();
    }

    /// <summary>
    /// 异步设置页面参数。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnParameterAsync()
    {
        await base.OnParameterAsync();
        if (pageRoute != PageRoute)
        {
            pageRoute = PageRoute;
            PageId = Context.Current?.Id;
            InitTable();
            await base.RefreshAsync();
        }
    }

    /// <summary>
    /// 构建页面内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildPage(RenderTreeBuilder builder)
    {
        if (Context.Current == null)
        {
            UI.Page404(builder, PageId);
            return;
        }

        var type = Utils.ConvertTo<ModuleType>(Context.Current.Target);
        if (type == ModuleType.IFrame)
        {
            builder.IFrame(Context.Current.Url);
            return;
        }

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
        var fields = Context?.Current?.Model?.Fields;
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