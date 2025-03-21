namespace Known.Entities;

/// <summary>
/// 系统消息实体类。
/// </summary>
[DisplayName("系统消息")]
public class SysMessage : EntityBase
{
    /// <summary>
    /// 取得或设置用户ID。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [DisplayName("用户ID")]
    public string UserId { get; set; }

    /// <summary>
    /// 取得或设置类型（收件、发件、删除）。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [DisplayName("类型")]
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置发件人。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [DisplayName("发件人")]
    public string MsgBy { get; set; }

    /// <summary>
    /// 取得或设置级别（普通、紧急）。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [DisplayName("级别")]
    public string MsgLevel { get; set; }

    /// <summary>
    /// 取得或设置分类。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("分类")]
    public string Category { get; set; }

    /// <summary>
    /// 取得或设置主题。
    /// </summary>
    [Required]
    [MaxLength(250)]
    [DisplayName("主题")]
    public string Subject { get; set; }

    /// <summary>
    /// 取得或设置内容。
    /// </summary>
    [Required]
    [DisplayName("内容")]
    public string Content { get; set; }

    /// <summary>
    /// 取得或设置附件。
    /// </summary>
    [MaxLength(500)]
    [DisplayName("附件")]
    public string FilePath { get; set; }

    /// <summary>
    /// 取得或设置是否Html。
    /// </summary>
    [DisplayName("是否Html")]
    public bool IsHtml { get; set; }

    /// <summary>
    /// 取得或设置状态（未读、已读）。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [DisplayName("状态")]
    public string Status { get; set; }

    /// <summary>
    /// 取得或设置业务ID。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("业务ID")]
    public string BizId { get; set; }
}