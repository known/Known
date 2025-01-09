namespace Known.Weixins;

/// <summary>
/// 微信配置信息类。
/// </summary>
public class WeixinConfigInfo
{
    /// <summary>
    /// 取得或设置公众号ID。
    /// </summary>
    public string GZHId { get; set; }

    /// <summary>
    /// 取得或设置微信公众号AppId。
    /// </summary>
    public string AppId { get; set; }

    /// <summary>
    /// 取得或设置微信公众号安全密钥。
    /// </summary>
    public string AppSecret { get; set; }

    /// <summary>
    /// 取得或设置微信公众绑定的服务器URL。
    /// </summary>
    public string RedirectUri { get; set; }
}