namespace Known.Weixins;

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