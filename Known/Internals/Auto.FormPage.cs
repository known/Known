namespace Known.Internals;

class AutoFormPage : BaseTabPage, IAutoPage
{
    private AutoDataService Service;
    private FormModel<Dictionary<string, object>> Model;

    [Parameter] public string PageId { get; set; }

    public async Task InitializeAsync()
    {
        Model = new FormModel<Dictionary<string, object>>(this);
        Model.Data = await Service.GetModelAsync("") ?? [];
        Model.OnSave = SaveModelAsync;
    }

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Service = await AutoDataService.CreateServiceAsync(this);
        Tab.AddTab(Language.BasicInfo, BuildForm);
        await InitializeAsync();
    }

    private void BuildForm(RenderTreeBuilder builder)
    {
        var param = Context?.Current?.GetAutoPageParameter();
        Model.Initialize(param?.Form);
        builder.Component<BaseEditForm<Dictionary<string, object>>>()
               .Set(c => c.Model, Model)
               .Build();
    }

    private Task<Result> SaveModelAsync(Dictionary<string, object> model)
    {
        var info = new UploadInfo<Dictionary<string, object>>(model);
        return Service.SaveModelAsync(info);
    }
}