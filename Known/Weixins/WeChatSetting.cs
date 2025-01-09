namespace Known.Weixins;

class WeChatSetting : BaseEditForm<WeixinInfo>
{
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        var data = await Admin.GetWeixinAsync("");
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
            builder.Form(Model);
            builder.FormButton(() => BuildAction(builder));
        });
    }

    protected override Task<Result> OnSaveAsync(WeixinInfo model)
    {
        return Admin.SaveWeixinAsync(model);
    }
}