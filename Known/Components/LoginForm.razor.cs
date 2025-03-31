namespace Known.Components;

/// <summary>
/// 登录表单组件类。
/// </summary>
public partial class LoginForm
{
    private LoginInfoForm form;
    private AntCaptcha captcha;
    private readonly CaptchaOption option = new() { SMSCount = 60 };
    private string FormStyle => IsCaptcha || Stations != null ? "" : "kui-nocaptcha";

    /// <summary>
    /// 取得或设置是否显示验证码。
    /// </summary>
    [Parameter] public bool IsCaptcha { get; set; }

    /// <summary>
    /// 取得或设置表单数据对象。
    /// </summary>
    [Parameter] public LoginFormInfo Model { get; set; }

    /// <summary>
    /// 取得或设置站别项目列表。
    /// </summary>
    [Parameter] public List<string> Stations { get; set; }

    /// <summary>
    /// 取得或设置发送短信验证码委托。
    /// </summary>
    [Parameter] public Func<string, Task<Result>> OnSendSMS { get; set; }

    /// <summary>
    /// 取得或设置登录操作委托。
    /// </summary>
    [Parameter] public Func<Task> OnLogin { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        if (OnSendSMS != null)
        {
            option.SMSValidate = () =>
            {
                if (string.IsNullOrWhiteSpace(Model.PhoneNo))
                    return Known.Result.Error(Language.Required("PhoneNo"));
                if (string.IsNullOrWhiteSpace(Model.Captcha))
                    return Known.Result.Error(Language.Required("Captcha"));
                if (!captcha.Validate(out string message))
                    return Known.Result.Error(message);

                return Known.Result.Success("");
            };
            option.SMSAction = async () =>
            {
                var result = await OnSendSMS?.Invoke(Model.PhoneNo);
                UI.Result(result);
                captcha?.Refresh();
            };
        }
    }

    private async Task OnFinish(EditContext context)
    {
        if (IsCaptcha && OnSendSMS == null)
        {
            if (!captcha.Validate(out string message))
            {
                await UI.Toast(message, StyleType.Error);
                return;
            }
        }

        Model.IsMobile = Context.IsMobile;
        await OnLogin?.Invoke();
    }
}