namespace Known.Pages;

[StreamRendering]
[Route("/page/{PageId}")]
public class AutoTablePage : BaseTablePage<Dictionary<string, object>>
{
    private IAutoService Service;
    private IModuleService Module;
    private bool isEditPage;
    private string pageId;
    private readonly Dictionary<string, object> defaultData = [];

    [Parameter] public string PageId { get; set; }

    public override async Task RefreshAsync()
    {
        if (isEditPage)
            InitTable();

        await base.RefreshAsync();
    }

    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        Service = await CreateServiceAsync<IAutoService>();
        Module = await CreateServiceAsync<IModuleService>();
    }

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

    public void New() => Table.NewForm(SaveModelAsync, defaultData);
    public void DeleteM() => Table.DeleteM(DeleteModelsAsync);
    public void Edit(Dictionary<string, object> row) => Table.EditForm(SaveModelAsync, row);
    public void Delete(Dictionary<string, object> row) => Table.Delete(DeleteModelsAsync, row);
    public async void Import() => await ShowImportAsync();
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