namespace Known.AI;

/// <summary>
/// AI聊天信息类。
/// </summary>
public class ChatInfo
{
    /// <summary>
    /// 构造函数。
    /// </summary>
    public ChatInfo()
    {
        Id = Utils.GetNextId();
        CreateTime = DateTime.Now;
    }

    /// <summary>
    /// 取得或设置ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置用户ID。
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// 取得或设置会话ID。
    /// </summary>
    public string SessionId { get; set; }

    /// <summary>
    /// 取得或设置助理应用ID。
    /// </summary>
    public string AgentId { get; set; }

    /// <summary>
    /// 取得或设置助理应用ID。
    /// </summary>
    [MaxLength(50)]
    [Column(Width = 120)]
    [DisplayName("AI助理")]
    public string AgentName { get; set; }

    /// <summary>
    /// 取得或设置创建人。
    /// </summary>
    [MaxLength(50)]
    [Column(Width = 100, IsQuery = true)]
    [DisplayName("创建人")]
    public string CreateBy { get; set; }

    /// <summary>
    /// 取得或设置创建时间。
    /// </summary>
    [Column(Width = 140)]
    [DisplayName("创建时间")]
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 取得或设置是否是发送信息。
    /// </summary>
    [Column(Width = 90)]
    [DisplayName("发/回")]
    public bool IsSend { get; set; }

    /// <summary>
    /// 取得或设置上下文内容。
    /// </summary>
    [Column]
    [DisplayName("上下文内容")]
    public string Context { get; set; }

    /// <summary>
    /// 取得或设置AI智能体信息。
    /// </summary>
    public AgentInfo Agent { get; set; }

    internal bool IsEdit { get; set; }
}