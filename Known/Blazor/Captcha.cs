using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class CaptchaOption
{
    public string ImgUrl { get; set; }
    public int SMSCount { get; set; }
    public Func<Task<Result>> SMSAction { get; set; }
}

public class Captcha : BaseComponent
{
    private readonly string id;
    private System.Timers.Timer timer;
    private string title;
    private string smsText;
    private int smsCount;
    private string code;
    private string lastCode;

    private bool IsSMS => Option != null && Option.SMSCount > 0;
    private bool IsRemoteImage => Option != null && !string.IsNullOrWhiteSpace(Option.ImgUrl);
    private bool IsLocalImage => !IsSMS && !IsRemoteImage;

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
            message = Context.Language["Captcha.NotValid"];
            return false;
        }
        return true;
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        title = Context.Language["Captcha.Refresh"];
        smsText = Context.Language["Captcha.Fetch"];
        if (IsSMS)
        {
            smsCount = Option.SMSCount;
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += Timer_Elapsed;
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (IsSMS)
            BuildSMS(builder);
        else if (IsRemoteImage)
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
        builder.Image().Src(Option.ImgUrl).Title(title).OnClick("this.src=this.src.split('?')[0]+'?t='+Math.random();").Close();
    }

    private void BuildCanvas(RenderTreeBuilder builder)
    {
        builder.Canvas().Id(id).Title(title).OnClick(this.Callback(CreateCode)).Close();
    }

    private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        if (smsCount > 0)
        {
            smsCount--;
            smsText = Context.Language["Captcha.Countdown"].Replace("{smsCount}", $"{smsCount}");
        }
        else
        {
            smsCount = Option.SMSCount;
            timer.Enabled = false;
            smsText = Context.Language["Captcha.Fetch"];
        }
        StateChanged();
    }

    private async void SendSMSCode()
    {
        if (smsCount < Option.SMSCount)
            return;

        if (Option.SMSAction == null)
            return;

        var result = await Option.SMSAction.Invoke();
        UI.Result(result, () =>
        {
            smsCount = Option.SMSCount;
            timer.Enabled = true;
        });
    }

    private void CreateCode() => code = Utils.GetCaptcha(4);
}