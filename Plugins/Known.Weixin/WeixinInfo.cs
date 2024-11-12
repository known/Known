namespace Known.Weixin;

/// <summary>
/// 微信信息配置类。
/// </summary>
public class WeixinInfo
{
    /// <summary>
    /// 取得或设置是否启用微信扫码关注公众号。
    /// </summary>
    public bool IsWeixinAuth { get; set; }

    /// <summary>
    /// 取得或设置系统微信用户信息。
    /// </summary>
    public SysWeixin User { get; set; }
}

/// <summary>
/// 微信模板消息业务类。
/// </summary>
public class WeixinTemplateInfo
{
    /// <summary>
    /// 取得或设置业务ID。
    /// </summary>
    public string BizId { get; set; }

    /// <summary>
    /// 取得或设置业务名称。
    /// </summary>
    public string BizName { get; set; }

    /// <summary>
    /// 取得或设置模板消息信息。
    /// </summary>
    public TemplateInfo Template { get; set; }
}