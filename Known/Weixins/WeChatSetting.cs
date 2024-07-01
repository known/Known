namespace Known.Weixins;

class WeChatSetting : BaseEditForm<WeixinInfo>
{
    private IWeixinService weixinService;

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        weixinService = await CreateServiceAsync<IWeixinService>();
        var data = await weixinService.GetWeixinAsync();
        Model = new FormModel<WeixinInfo>(Context)
        {
            IsView = true,
            LabelSpan = 4,
            WrapperSpan = 10,
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
        return weixinService.SaveWeixinAsync(model);
    }
}