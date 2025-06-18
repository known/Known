namespace Known.Components;

/// <summary>
/// 注册用户表单组件类。
/// </summary>
public partial class RegisterForm
{
    private AntCaptcha captcha;

    /// <summary>
    /// 取得或设置表单数据对象。
    /// </summary>
    [Parameter] public RegisterFormInfo Model { get; set; }

    /// <summary>
    /// 取得或设置注册操作委托。
    /// </summary>
    [Parameter] public Func<Task> OnRegister { get; set; }

    private Task OnFinish(EditContext context)
    {
        if (!captcha.Validate(out string message))
        {
            UI.Error(message);
            return Task.CompletedTask;
        }

        return OnRegister?.Invoke();
    }
}