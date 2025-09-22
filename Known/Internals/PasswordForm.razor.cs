namespace Known.Internals;

/// <summary>
/// 修改密码表单组件。
/// </summary>
public partial class PasswordForm
{
    private readonly PwdFormInfo Model = new();
    private TypeForm form;
    private string strengthValue = "";
    private string strengthText = "";
    private string strengthTips = Language.PleaseInputPassword;

    /// <summary>
    /// 取得或设置提示文本。
    /// </summary>
    [Parameter] public string TipText { get; set; }

    /// <summary>
    /// 取得或设置提交表单委托。
    /// </summary>
    [Parameter] public Func<PwdFormInfo, Task> OnSave { get; set; }

    private void OnNewPwdInput(ChangeEventArgs args)
    {
        var password = args?.Value?.ToString() ?? string.Empty;
        CheckPasswordStrength(password);
    }

    private void OnUpdate()
    {
        if (!form.Validate())
            return;

        if (Model.NewPwd != Model.NewPwd1)
        {
            UI.Error(Language.TipPwdNotEqual);
            return;
        }

        OnSave?.Invoke(Model);
    }

    private void CheckPasswordStrength(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            strengthValue = "";
            strengthTips = Language.PleaseInputPassword;
            return;
        }

        var validator = new PasswordValidator();
        validator.Validate(Config.System, password);
        if (validator.Strength < 3)
            strengthValue = "weak";//strengthText = "密码强度：弱";
        else if (validator.Strength < 5)
            strengthValue = "medium";//strengthText = "密码强度：中";
        else
            strengthValue = "strong";//strengthText = "密码强度：强";
        strengthTips = validator.Message;
    }
}