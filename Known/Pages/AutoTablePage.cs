namespace Known.Pages;

[StreamRendering]
[Route("/page/{PageId}")]
public class AutoTablePage : BaseTablePage<Dictionary<string, object>>
{
    private IAutoService autoService;
    private IModuleService moduleService;
    private bool isEditPage;

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
        autoService = await CreateServiceAsync<IAutoService>();
        moduleService = await CreateServiceAsync<IModuleService>();
        InitTable();
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
        if (CurrentUser != null && CurrentUser.UserName == "admin")
        {
            builder.Div("kui-page-designer", () =>
            {
                builder.Icon("fa fa-edit", this.Callback<MouseEventArgs>(OnEditPage));
            });
        }
    }

    public void New() => Table.NewForm(SaveModelAsync, []);
    public void DeleteM() => Table.DeleteM(DeleteModelsAsync);
    public void Edit(Dictionary<string, object> row) => Table.EditForm(SaveModelAsync, row);
    public void Delete(Dictionary<string, object> row) => Table.Delete(DeleteModelsAsync, row);
    public void Import() => ShowImportForm();
    public async void Export() => await ExportDataAsync();

    private void InitTable()
    {
        Table.Initialize(this);
        Table.OnQuery = OnQueryModelsAsync;
        Table.Criteria.Clear();
    }

    private Task<PagingResult<Dictionary<string, object>>> OnQueryModelsAsync(PagingCriteria criteria)
    {
        criteria.Parameters[nameof(PageId)] = PageId;
        return autoService.QueryModelsAsync(criteria);
    }

    private Task<Result> DeleteModelsAsync(List<Dictionary<string, object>> models)
    {
        var info = new AutoInfo<List<Dictionary<string, object>>>
        {
            PageId = PageId,
            Data = models
        };
        return autoService.DeleteModelsAsync(info);
    }

    private Task<Result> SaveModelAsync(UploadInfo<Dictionary<string, object>> info)
    {
        info.PageId = PageId;
        return autoService.SaveModelAsync(info);
    }

    private async void OnEditPage(MouseEventArgs args)
    {
        isEditPage = true;
        var form = new FormModel<SysModule>(this)
        {
            Data = await moduleService.GetModuleAsync(Context.Current.Id),
            OnSave = moduleService.SaveModuleAsync,
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