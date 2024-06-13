namespace Known.Weixins;

class WeChatSetting : BaseEditForm<WeixinInfo>
{
    private IWeixinService weixinService;

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        weixinService = await CreateServiceAsync<IWeixinService>();
        Model = new FormModel<WeixinInfo>(Context)
        {
            IsView = true,
            LabelSpan = 4,
            WrapperSpan = 10
        };
        Model.AddRow().AddColumn(c => c.IsWeixinAuth);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            var data = await weixinService.GetWeixinAsync();
            data ??= new WeixinInfo();
            Model.Data = data;
        }
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