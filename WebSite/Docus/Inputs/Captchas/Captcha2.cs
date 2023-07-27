namespace WebSite.Docus.Inputs.Captchas;

class Captcha2 : BaseComponent
{
    private Captcha? captcha;
    private string? message;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Field<Captcha>("Captcha", true)
               .Set(f => f.Placeholder, "验证码")
               .Build(value => captcha = value);

        builder.Button("验证", Callback(OnCheck), StyleType.Primary);
        builder.Div("tips", message);
    }

    private void OnCheck() => captcha?.Validate(out message);
}