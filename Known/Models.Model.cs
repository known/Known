namespace Known;

/// <summary>
/// 动态组件信息类。
/// </summary>
public class ComponentInfo
{
    /// <summary>
    /// 取得或设置组件排序。
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 取得或设置组件类型。
    /// </summary>
    public Type Type { get; set; }

    /// <summary>
    /// 取得或设置组件参数。
    /// </summary>
    public Dictionary<string, object> Parameters { get; set; }
}

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