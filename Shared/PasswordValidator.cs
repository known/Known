using System.Text.RegularExpressions;

namespace Known;

/// <summary>
/// 密码验证器类。
/// </summary>
public class PasswordValidator
{
    private readonly List<string> tips = [];

    /// <summary>
    /// 取得或设置密码强度。
    /// </summary>
    public int Strength { get; private set; }

    /// <summary>
    /// 取得或设置验证消息。
    /// </summary>
    public string Message { get; private set; }

    /// <summary>
    /// 验证密码。
    /// </summary>
    /// <param name="info">系统配置信息。</param>
    /// <param name="password">密码。</param>
    /// <param name="isComplexity">是否验证复杂度。</param>
    public void Validate(SystemInfo info, string password, bool isComplexity = false)
    {
        Strength = 0;
        if (string.IsNullOrWhiteSpace(password))
        {
            Message = Language.PleaseInputPassword;
            return;
        }

        if (isComplexity)
        {
            var complexity = Utils.ConvertTo<PasswordComplexity>(info.PwdComplexity);
            if (complexity == PasswordComplexity.High || complexity == PasswordComplexity.Middle || complexity == PasswordComplexity.Low)
                CheckLength(password, info.PwdLength);
            if (complexity == PasswordComplexity.High || complexity == PasswordComplexity.Middle)
                CheckCharAndNumber(password);
            if (complexity == PasswordComplexity.High)
                CheckSpecialChar(password);
        }
        else
        {
            CheckLength(password, info.PwdLength);
            CheckCharAndNumber(password);
            CheckSpecialChar(password);
        }

        if (tips.Count > 0 && Strength < 5)
        {
            if (!string.IsNullOrWhiteSpace(Message))
                Message += "，";
            if (isComplexity)
                Message += "必须";
            Message += $"包含{string.Join("、", tips)}。";
        }
    }

    private void CheckLength(string password, int? length)
    {
        if (password.Length < length)
            Message = $"至少{length}个字符";
        else
            Strength += 1;
    }

    private void CheckCharAndNumber(string password)
    {
        CheckPassword(password, "[a-z]", "小写");
        CheckPassword(password, "[A-Z]", "大写");
        CheckPassword(password, "[0-9]", "数字");
    }

    private void CheckSpecialChar(string password)
    {
        CheckPassword(password, "[^A-Za-z0-9]", "特殊字符");
    }

    private void CheckPassword(string password, string pattern, string tip)
    {
        if (Regex.IsMatch(password, pattern))
            Strength += 1;
        else
            tips.Add(tip);
    }
}