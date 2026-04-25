namespace Known.AI;

/// <summary>
/// AI模型信息类。
/// </summary>
public class ModelInfo
{
    /// <summary>
    /// 取得或设置聊天模型类型。
    /// </summary>
    public ChatType Type { get; set; }

    /// <summary>
    /// 取得或设置AI模型。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("模型")]
    public string Model { get; set; }

    /// <summary>
    /// 取得或设置请求地址。
    /// </summary>
    [MaxLength(500)]
    [DisplayName("请求地址")]
    public string EndPoint { get; set; }

    /// <summary>
    /// 取得或设置模型ApiKey。
    /// </summary>
    [MaxLength(500)]
    [DisplayName("ApiKey")]
    public string ApiKey { get; set; }
}