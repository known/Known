using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.BootBlazor.Components;

public class BootCaptcha : BootstrapBlazor.Components.BootstrapInput<string>
{
    private Captcha captcha;

    [Parameter] public CaptchaOption Option { get; set; }

    public bool Validate(out string message) => captcha.Validate(Value, out message);

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);
        builder.Component<Captcha>().Set(c => c.Option, Option).Build(value => captcha = value);
    }
}