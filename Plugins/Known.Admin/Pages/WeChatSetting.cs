namespace Known.Pages;

class WeChatSetting : BaseEditForm<WeixinInfo>
{
    private IWeixinService Service;

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Service = await CreateServiceAsync<IWeixinService>();
        var data = await Service.GetWeixinAsync("");
        Model = new FormModel<WeixinInfo>(this)
        {
            Class = "kui-system",
            IsView = true,
            Data = data ?? new WeixinInfo()
        };
        Model.AddRow().AddColumn(c => c.IsWeixinAuth);
    }

    protected override void BuildFormContent(RenderTreeBuilder builder)
    {
        builder.FormPage(() =>
        {
            UI.BuildForm(builder, Model);
            builder.FormButton(() => BuildAction(builder));
        });
    }

    protected override Task<Result> OnSaveAsync(WeixinInfo model)
    {
        return Service.SaveWeixinAsync(model);
    }
}