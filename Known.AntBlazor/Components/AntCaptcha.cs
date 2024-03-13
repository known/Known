namespace Known.AntBlazor.Components;

public class AntCaptcha : Input<string>
{
    private KCaptcha captcha;

    public AntCaptcha()
    {
        Class = "ant-captcha";
    }

    [Parameter] public CaptchaOption Option { get; set; }

    public bool Validate(out string message) => captcha.Validate(Value, out message);

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);
        builder.Component<KCaptcha>().Set(c => c.Option, Option).Build(value => captcha = value);
    }
}