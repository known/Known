using System.Text.Json.Serialization;

namespace Known.Winxins;

public class AuthorizeToken
{
    public string AccessToken { get; set; }
    public int ExpiresIn { get; set; }
    public string RefreshToken { get; set; }
    public string OpenId { get; set; }
    public string Scope { get; set; }
    public int IsSnapshotUser { get; set; }
    public string UnionId { get; set; }
}

public class AuthorizeRefreshToken
{
    public string AccessToken { get; set; }
    public int ExpiresIn { get; set; }
    public string RefreshToken { get; set; }
    public string OpenId { get; set; }
    public string Scope { get; set; }
}

public class TemplateInfo
{
    [JsonPropertyName("touser")]
    public string ToUser { get; set; }
    [JsonPropertyName("template_id")]
    public string TemplateId { get; set; }
    [JsonPropertyName("client_msg_id")]
    public string ClientMsgId { get; set; }
    [JsonPropertyName("url")]
    public string Url { get; set; }
    [JsonPropertyName("miniprogram")]
    public MiniProgramInfo MiniProgram { get; set; }
    [JsonPropertyName("data")]
    public object Data { get; set; }
}

public class MiniProgramInfo
{
    [JsonPropertyName("appid")]
    public string AppId { get; set; }
    [JsonPropertyName("pagepath")]
    public string PagePath { get; set; }
}