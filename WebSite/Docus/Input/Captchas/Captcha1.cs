namespace WebSite.Docus.Input.Captchas;

[Title("默认示例")]
class Captcha1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Field<Captcha>("Captcha", true)
               .Set(f => f.Placeholder, "验证码")
               .Build();
    }
}