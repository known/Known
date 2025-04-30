﻿namespace Known.Internals;

/// <summary>
/// 验证码组件类。
/// </summary>
class KCaptcha : BaseComponent
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
    private string SmsText => Language[Language.CaptchaFetch];

    /// <summary>
    /// 构造函数，创建一个验证码组件类的实例。
    /// </summary>
    public KCaptcha()
    {
        id = Utils.GetNextId();
        CreateCode();
        Option = new CaptchaOption();
    }

    /// <summary>
    /// 取得或设置验证码选项。
    /// </summary>
    [Parameter] public CaptchaOption Option { get; set; }

    /// <summary>
    /// 校验验证码。
    /// </summary>
    /// <param name="value">验证码字符串。</param>
    /// <param name="message">验证错误信息。</param>
    /// <returns>是否验证成功。</returns>
    public bool Validate(string value, out string message)
    {
        message = string.Empty;
        if (!code.Equals(value, StringComparison.OrdinalIgnoreCase))
        {
            message = Language[Language.CaptchaNotValid];
            return false;
        }
        return true;
    }

    /// <summary>
    /// 刷新验证码。
    /// </summary>
    public void Refresh()
    {
        CreateCode();
        StateChanged();
    }

    /// <summary>
    /// 异步初始化验证码组件。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        title = Language[Language.CaptchaRefresh];
        smsText = SmsText;
        if (IsSMS)
        {
            smsCount = Option.SMSCount;
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += Timer_Elapsed;
        }
    }

    /// <summary>
    /// 呈现验证码组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (IsSMS)
            BuildSMS(builder);
        else if (IsRemoteImage)
            BuildImage(builder);
        else
            BuildCanvas(builder);
    }

    /// <summary>
    /// 验证码组件呈现后，调用js绘制验证码。
    /// </summary>
    /// <param name="firstRender">是否首次呈现。</param>
    /// <returns></returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (IsLocalImage && (firstRender || code != lastCode))
        {
            lastCode = code;
            await JS.CaptchaAsync(id, code);
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    /// <summary>
    /// 释放验证码组件对象。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnDisposeAsync()
    {
        await base.OnDisposeAsync();
        timer?.Dispose();
    }

    private void BuildSMS(RenderTreeBuilder builder)
    {
        builder.Span().Class("btn-sms").OnClick(this.Callback(SendSMSCode)).Child(smsText);
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
            smsText = Language[Language.CaptchaCountdown].Replace("{SmsCount}", $"{smsCount}");
        }
        else
        {
            smsCount = Option.SMSCount;
            timer.Enabled = false;
            smsText = SmsText;
        }
        StateChangedAsync();
    }

    private void SendSMSCode()
    {
        if (smsCount < Option.SMSCount)
            return;

        if (Option.SMSValidate == null || Option.SMSAction == null)
            return;

        var result = Option.SMSValidate.Invoke();
        if (!result.IsValid)
        {
            UI.Error(result.Message);
            return;
        }

        smsCount = Option.SMSCount;
        timer.Enabled = true;
        Option.SMSAction.Invoke();
    }

    private void CreateCode() => code = Utils.GetCaptcha(4);
}