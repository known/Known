using System.Text.Json.Serialization;

namespace Known.Weixin;

/// <summary>
/// 微信票据信息类。
/// </summary>
public class TicketInfo
{
    /// <summary>
    /// 取得或设置票据。
    /// </summary>
    public string Ticket { get; set; }

    /// <summary>
    /// 取得或设置过期时长。
    /// </summary>
    public int ExpireSeconds { get; set; }

    /// <summary>
    /// 取得或设置URL。
    /// </summary>
    public string Url { get; set; }
}

/// <summary>
/// 微信认证票据类。
/// </summary>
public class AuthorizeToken
{
    /// <summary>
    /// 取得或设置访问Token。
    /// </summary>
    public string AccessToken { get; set; }

    /// <summary>
    /// 取得或设置过期时长。
    /// </summary>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// 取得或设置刷新Token。
    /// </summary>
    public string RefreshToken { get; set; }

    /// <summary>
    /// 取得或设置用户OpenId。
    /// </summary>
    public string OpenId { get; set; }

    /// <summary>
    /// 取得或设置范围。
    /// </summary>
    public string Scope { get; set; }

    /// <summary>
    /// 取得或设置是否快照。
    /// </summary>
    public int IsSnapshotUser { get; set; }

    /// <summary>
    /// 取得或设置统一ID。
    /// </summary>
    public string UnionId { get; set; }
}

/// <summary>
/// 微信认证刷新Token信息类。
/// </summary>
public class AuthorizeRefreshToken
{
    /// <summary>
    /// 取得或设置访问Token。
    /// </summary>
    public string AccessToken { get; set; }

    /// <summary>
    /// 取得或设置过期时长。
    /// </summary>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// 取得或设置刷新Token。
    /// </summary>
    public string RefreshToken { get; set; }

    /// <summary>
    /// 取得或设置用户OpenId。
    /// </summary>
    public string OpenId { get; set; }

    /// <summary>
    /// 取得或设置范围。
    /// </summary>
    public string Scope { get; set; }
}

/// <summary>
/// 微信模板消息信息类。
/// </summary>
public class TemplateInfo
{
    /// <summary>
    /// 取得或设置接收人OpenId。
    /// </summary>
    [JsonPropertyName("touser")]
    public string ToUser { get; set; }

    /// <summary>
    /// 取得或设置模板消息ID。
    /// </summary>
    [JsonPropertyName("template_id")]
    public string TemplateId { get; set; }

    /// <summary>
    /// 取得或设置客户端消息ID。
    /// </summary>
    [JsonPropertyName("client_msg_id")]
    public string ClientMsgId { get; set; }

    /// <summary>
    /// 取得或设置模板消息调转的URL。
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; }

    /// <summary>
    /// 取得或设置小程序信息。
    /// </summary>
    [JsonPropertyName("miniprogram")]
    public MiniProgramInfo MiniProgram { get; set; }

    /// <summary>
    /// 取得或设置模板消息数据对象。
    /// </summary>
    [JsonPropertyName("data")]
    public object Data { get; set; }
}

/// <summary>
/// 微信小程序信息类。
/// </summary>
public class MiniProgramInfo
{
    /// <summary>
    /// 取得或设置AppId。
    /// </summary>
    [JsonPropertyName("appid")]
    public string AppId { get; set; }

    /// <summary>
    /// 取得或设置页面地址。
    /// </summary>
    [JsonPropertyName("pagepath")]
    public string PagePath { get; set; }
}

/// <summary>
/// 微信模板消息数据信息类。
/// </summary>
public class TemplateData
{
    /// <summary>
    /// 取得或设置模板数据。
    /// </summary>
    [JsonPropertyName("value")]
    public string Value { get; set; }

    /// <summary>
    /// 创建一个微信模板消息数据对象。
    /// </summary>
    /// <param name="value">模板数据。</param>
    /// <returns>模板消息数据对象。</returns>
    public static TemplateData Create(string value)
    {
        if (!string.IsNullOrWhiteSpace(value) && value.Length > 20)
            value = value.Substring(0, 20);

        return new TemplateData { Value = value };
    }
}