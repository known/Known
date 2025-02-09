namespace Known.Entities;

/// <summary>
/// 用户消息实体类。
/// </summary>
public class SysMessage : EntityBase
{
    /// <summary>
    /// 取得或设置用户ID。
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string UserId { get; set; }

    /// <summary>
    /// 取得或设置类型（收件、发件、删除）。
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置发件人。
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string MsgBy { get; set; }

    /// <summary>
    /// 取得或设置级别（普通、紧急）。
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string MsgLevel { get; set; }

    /// <summary>
    /// 取得或设置分类。
    /// </summary>
    [MaxLength(50)]
    public string Category { get; set; }

    /// <summary>
    /// 取得或设置主题。
    /// </summary>
    [Required]
    [MaxLength(250)]
    public string Subject { get; set; }

    /// <summary>
    /// 取得或设置内容。
    /// </summary>
    [Required]
    public string Content { get; set; }

    /// <summary>
    /// 取得或设置附件。
    /// </summary>
    [MaxLength(500)]
    public string FilePath { get; set; }

    /// <summary>
    /// 取得或设置是否Html。
    /// </summary>
    public bool IsHtml { get; set; }

    /// <summary>
    /// 取得或设置状态（未读、已读）。
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Status { get; set; }

    /// <summary>
    /// 取得或设置业务ID。
    /// </summary>
    [MaxLength(50)]
    public string BizId { get; set; }
}