namespace Known.BootBlazor.Components;

public class BootCaptcha : BootstrapInput<string>
{
    private KCaptcha captcha;

    [Parameter] public Known.Components.CaptchaOption Option { get; set; }

    public bool Validate(out string message) => captcha.Validate(Value, out message);

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);
        builder.Component<KCaptcha>().Set(c => c.Option, Option).Build(value => captcha = value);
    }
}