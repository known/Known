using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Known.Entities;

/// <summary>
/// 用户消息实体类。
/// </summary>
public class SysMessage : EntityBase
{
    /// <summary>
    /// 取得或设置用户ID。
    /// </summary>
    [DisplayName("用户ID")]
    [Required(ErrorMessage = "用户ID不能为空！")]
    [MaxLength(50)]
    public string UserId { get; set; }

    /// <summary>
    /// 取得或设置类型（收件、发件、删除）。
    /// </summary>
    [DisplayName("类型")]
    [Required(ErrorMessage = "类型不能为空！")]
    [MaxLength(50)]
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置发件人。
    /// </summary>
    [DisplayName("发件人")]
    [Required(ErrorMessage = "发件人不能为空！")]
    [MaxLength(50)]
    public string MsgBy { get; set; }

    /// <summary>
    /// 取得或设置级别（普通、紧急）。
    /// </summary>
    [DisplayName("级别")]
    [Required(ErrorMessage = "级别不能为空！")]
    [MaxLength(50)]
    public string MsgLevel { get; set; }

    /// <summary>
    /// 取得或设置分类。
    /// </summary>
    [DisplayName("分类")]
    [MaxLength(50)]
    public string Category { get; set; }

    /// <summary>
    /// 取得或设置主题。
    /// </summary>
    [DisplayName("主题")]
    [Required(ErrorMessage = "主题不能为空！")]
    [MaxLength(250)]
    public string Subject { get; set; }

    /// <summary>
    /// 取得或设置内容。
    /// </summary>
    [Required(ErrorMessage = "内容不能为空！")]
    [DisplayName("内容")]
    public string Content { get; set; }

    /// <summary>
    /// 取得或设置附件。
    /// </summary>
    [DisplayName("附件")]
    [MaxLength(500)]
    public string FilePath { get; set; }

    /// <summary>
    /// 取得或设置是否Html。
    /// </summary>
    [DisplayName("是否Html")]
    public bool IsHtml { get; set; }

    /// <summary>
    /// 取得或设置状态（未读、已读）。
    /// </summary>
    [DisplayName("状态")]
    [Required(ErrorMessage = "状态不能为空！")]
    [MaxLength(50)]
    public string Status { get; set; }

    /// <summary>
    /// 取得或设置业务ID。
    /// </summary>
    [DisplayName("业务ID")]
    [MaxLength(50)]
    public string BizId { get; set; }
}