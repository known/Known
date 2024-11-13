namespace Known.Admin.Entities;

/// <summary>
/// 系统日志实体类。
/// </summary>
public class SysLog : EntityBase
{
    /// <summary>
    /// 取得或设置操作类型。
    /// </summary>
    [Category(nameof(LogType))]
    [Required]
    [MaxLength(50)]
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置操作对象。
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Target { get; set; }

    /// <summary>
    /// 取得或设置操作内容。
    /// </summary>
    public string Content { get; set; }
}