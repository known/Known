using System.Text.RegularExpressions;

namespace Known.Internals;

public partial class PasswordForm
{
    private PwdFormInfo Model = new();
    private TypeForm form;
    private string strengthValue = "";
    private string strengthText = "";
    private string strengthTips = "请输入密码";

    /// <summary>
    /// 取得或设置提示文本。
    /// </summary>
    [Parameter] public string TipText { get; set; }

    private void OnNewPwdInput(ChangeEventArgs args)
    {
        var password = args?.Value?.ToString() ?? string.Empty;
        CheckPasswordStrength(password);
    }

    private void OnUpdate()
    {
        if (!form.Validate())
            return;
    }

    private void CheckPasswordStrength(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            strengthValue = "";
            strengthTips = "请输入密码";
            return;
        }

        int strength = 0;
        var tip = string.Empty;
        var tips = new List<string>();
        if (password.Length < 6)
            tip = "至少6个字符，";
        else
            strength += 1;

        if (Regex.IsMatch(password, "[a-z]"))
            strength += 1;
        else
            tips.Add("小写");

        if (Regex.IsMatch(password, "[A-Z]"))
            strength += 1;
        else
            tips.Add("大写");

        if (Regex.IsMatch(password, "[0-9]"))
            strength += 1;
        else
            tips.Add("数字");

        if (Regex.IsMatch(password, "[^A-Za-z0-9]"))
            strength += 1;
        else
            tips.Add("特殊字符");

        if (strength < 3)
            strengthValue = "weak";//strengthText = "密码强度：弱";
        else if (strength < 5)
            strengthValue = "medium";//strengthText = "密码强度：中";
        else
            strengthValue = "strong";//strengthText = "密码强度：强";

        if (tips.Count > 0 && strength < 5)
            strengthTips = $"{tip}包含{string.Join("、", tips)}。";
        else
            strengthTips = "";
    }
}