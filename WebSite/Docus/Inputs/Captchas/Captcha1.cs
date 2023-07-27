namespace WebSite.Docus.Inputs.Captchas;

class Captcha1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Field<Captcha>("Captcha", true)
               .Set(f => f.Placeholder, "验证码")
               .Build();
    }
}