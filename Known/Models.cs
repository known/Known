namespace Known;

/// <summary>
/// 验证码选项类。
/// </summary>
public class CaptchaOption
{
    /// <summary>
    /// 取得或设置验证码图片URL。
    /// </summary>
    public string ImgUrl { get; set; }

    /// <summary>
    /// 取得或设置短信验证码倒计时长度。
    /// </summary>
    public int SMSCount { get; set; }

    /// <summary>
    /// 取得或设置短信验证码验证委托。
    /// </summary>
    public Func<Result> SMSValidate { get; set; }

    /// <summary>
    /// 取得或设置短信验证码发送委托。
    /// </summary>
    public Action SMSAction { get; set; }
}