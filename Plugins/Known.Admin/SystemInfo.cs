namespace Known;

/// <summary>
/// 系统信息类。
/// </summary>
public class SystemInfo
{
    /// <summary>
    /// 取得或设置企业编码。
    /// </summary>
    public string CompNo { get; set; }

    /// <summary>
    /// 取得或设置企业名称。
    /// </summary>
    public string CompName { get; set; }

    /// <summary>
    /// 取得或设置系统名称。
    /// </summary>
    public string AppName { get; set; }

    /// <summary>
    /// 取得或设置产品ID。
    /// </summary>
    public string ProductId { get; set; }

    /// <summary>
    /// 取得或设置产品Key。
    /// </summary>
    public string ProductKey { get; set; }

    /// <summary>
    /// 取得或设置系统用户默认密码。
    /// </summary>
    public string UserDefaultPwd { get; set; }

    /// <summary>
    /// 取得或设置系统登录是否验证图片验证码。
    /// </summary>
    public bool IsLoginCaptcha { get; set; }
}