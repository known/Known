namespace Known.Pages;

/// <summary>
/// 无代码表格页面组件类。
/// </summary>
[StreamRendering]
[Route("/page/{PageId}")]
public class AutoTablePage : BaseTablePage<Dictionary<string, object>>
{
    private IAutoService Service;
    private IModuleService Module;
    private bool isEditPage;
    private string pageId;
    private readonly Dictionary<string, object> defaultData = [];

    /// <summary>
    /// 取得或设置页面模块ID。
    /// </summary>
    [Parameter] public string PageId { get; set; }

    /// <summary>
    /// 异步刷新页面。
    /// </summary>
    /// <returns></returns>
    public override async Task RefreshAsync()
    {
        if (isEditPage)
            InitTable();

        await base.RefreshAsync();
    }

    /// <summary>
    /// 异步初始化页面。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        Service = await CreateServiceAsync<IAutoService>();
        Module = await CreateServiceAsync<IModuleService>();
    }

    /// <summary>
    /// 异步设置页面参数。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnParameterAsync()
    {
        await base.OnParameterAsync();
        if (pageId != PageId)
        {
            pageId = PageId;
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
            UI.Build404Page(builder, PageId);
            return;
        }

        var type = Utils.ConvertTo<ModuleType>(Context.Current.Target);
        if (type == ModuleType.IFrame)
        {
            builder.IFrame(Context.Current.Url);
            return;
        }

        base.BuildPage(builder);
        if (CurrentUser != null && CurrentUser.IsSystemAdmin())
        {
            builder.Div("kui-page-designer", () =>
            {
                builder.Icon("fa fa-edit", this.Callback<MouseEventArgs>(OnEditPage));
            });
        }
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
    public async void Import() => await ShowImportAsync();

    /// <summary>
    /// 导出表格数据。
    /// </summary>
    public async void Export() => await ExportDataAsync();

    private void InitTable()
    {
        Table.Initialize(this);
        Table.OnQuery = OnQueryModelsAsync;
        Table.Criteria.Clear();
        foreach (var item in Context?.Current?.Model?.Fields)
        {
            defaultData[item.Id] = null;
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

    private async void OnEditPage(MouseEventArgs args)
    {
        isEditPage = true;
        var form = new FormModel<SysModule>(this)
        {
            Data = await Module.GetModuleAsync(Context.Current.Id),
            OnSave = Module.SaveModuleAsync,
            //OnSaved = data => Context.Current = new MenuInfo(data)
        };
        var model = new DialogModel
        {
            Title = $"{Language["Designer.EditPage"]} - {PageName}",
            Maximizable = true,
            Width = 1200,
            Content = b => b.Component<ModuleForm>().Set(c => c.Model, form).Set(c => c.IsPageEdit, true).Build()
        };
        UI.ShowDialog(model);
        form.OnClose = async () =>
        {
            isEditPage = false;
            await model.OnClose?.Invoke();
        };
    }
}