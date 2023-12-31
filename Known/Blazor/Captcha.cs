using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class CaptchaOption
{
    public string ImgUrl { get; set; }
    public bool IsSMS { get; set; }
    public int SMSTimeout { get; set; }
    public Func<Task> SMSAction { get; set; }
}

public class Captcha : BaseComponent
{
    private const string title = "点击图片刷新";
    private readonly string id;
    private System.Timers.Timer timer;
    private string smsText = "获取验证码";
    private int smsCounter;
    private string code;
    private string lastCode;

    private bool IsLocalImage => Option == null || (!Option.IsSMS && string.IsNullOrWhiteSpace(Option.ImgUrl));

    public Captcha()
    {
        id = Utils.GetGuid();
        CreateCode();
        Option = new CaptchaOption();
    }

    [Parameter] public CaptchaOption Option { get; set; }

    public bool Validate(string value, out string message)
    {
        message = string.Empty;
        if (!code.Equals(value, StringComparison.OrdinalIgnoreCase))
        {
            message = "验证码不正确！";
            return false;
        }
        return true;
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        if (Option != null && Option.IsSMS)
        {
            smsCounter = Option.SMSTimeout;
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += Timer_Elapsed;
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Option != null && Option.IsSMS)
            BuildSMS(builder);
        else if (Option != null && !string.IsNullOrWhiteSpace(Option.ImgUrl))
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

    protected override ValueTask DisposeAsync(bool disposing)
    {
        timer?.Dispose();
        return base.DisposeAsync(disposing);
    }

    private void BuildSMS(RenderTreeBuilder builder)
    {
        builder.Span().Class("btn-sms").OnClick(this.Callback(SendSMSCode)).Text(smsText).Close();
    }

    private void BuildImage(RenderTreeBuilder builder)
    {
        builder.Image().Src(Option.ImgUrl).Title(title).OnClick("this.src=this.src;").Close();
    }

    private void BuildCanvas(RenderTreeBuilder builder)
    {
        builder.Canvas().Id(id).Title(title).OnClick(this.Callback(CreateCode)).Close();
    }

    private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        if (smsCounter > 0)
        {
            smsCounter--;
            smsText = $"{smsCounter}秒后重新获取";
        }
        else
        {
            smsCounter = Option.SMSTimeout;
            timer.Enabled = false;
            smsText = "获取验证码";
        }
        StateChanged();
    }

    private async void SendSMSCode()
    {
        if (smsCounter < Option.SMSTimeout)
            return;

        if (Option.SMSAction != null)
            await Option.SMSAction.Invoke();

        UI.Toast("获取成功！");
        smsCounter = Option.SMSTimeout;
        timer.Enabled = true;
    }

    private void CreateCode() => code = Utils.GetCaptcha(4);
}