namespace Known.AI;

/// <summary>
/// AI智能助理会话聊天实体类。
/// </summary>
[DisplayName("AI聊天记录")]
public class SysChat : EntityBase
{
    /// <summary>
    /// 取得或设置用户ID。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [DisplayName("用户ID")]
    public string UserId { get; set; }

    /// <summary>
    /// 取得或设置会话ID。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [DisplayName("会话ID")]
    public string SessionId { get; set; }

    /// <summary>
    /// 取得或设置助理应用ID。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [DisplayName("应用ID")]
    public string AgentId { get; set; }

    /// <summary>
    /// 取得或设置助理名称。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("助理名称")]
    public string AgentName { get; set; }

    /// <summary>
    /// 取得或设置是否是发送信息。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [DisplayName("是否用户发送")]
    public bool IsSend { get; set; }

    /// <summary>
    /// 取得或设置上下文内容。
    /// </summary>
    [DisplayName("消息内容")]
    public string Context { get; set; }
}