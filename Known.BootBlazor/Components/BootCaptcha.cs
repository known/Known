using BootstrapBlazor.Components;
using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.BootBlazor.Components;

public class BootCaptcha : BootstrapInput<string>
{
    private const string title = "点击图片刷新";
    private readonly string id;
    private string code;
    private string lastCode;

    public BootCaptcha()
    {
        id = Utils.GetGuid();
        CreateCode();
    }

    private bool IsLocalImage => !IsSMS && string.IsNullOrWhiteSpace(ImgUrl);

    [Inject] private JSService JS { get; set; }
    [Parameter] public bool IsSMS { get; set; }
    [Parameter] public string ImgUrl { get; set; }

    public bool Validate(out string message)
    {
        message = string.Empty;
        if (!code.Equals(Value, StringComparison.OrdinalIgnoreCase))
        {
            message = "验证码不正确！";
            return false;
        }
        return true;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);
        if (IsSMS)
            BuildSMS(builder);
        else if (!string.IsNullOrWhiteSpace(ImgUrl))
            BuildImage(builder);
        else
            BuildCanvas(builder);
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (IsLocalImage && (firstRender || code != lastCode))
        {
            lastCode = code;
            JS.Captcha(id, code);
        }
        return base.OnAfterRenderAsync(firstRender);
    }

    private void BuildSMS(RenderTreeBuilder builder)
    {
        builder.Span("btn-sms", "获取验证码");
    }

    private void BuildImage(RenderTreeBuilder builder)
    {
        builder.Image().Src(ImgUrl).Title(title).OnClick("alert(this.src);").Close();
    }

    private void BuildCanvas(RenderTreeBuilder builder)
    {
        builder.Canvas().Id(id).Title(title).OnClick(this.Callback(CreateCode)).Close();
    }

    private void CreateCode() => code = Utils.GetCaptcha(4);
}