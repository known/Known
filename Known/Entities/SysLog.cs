namespace Known.Entities;

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
    [Column(IsQuery = true, Type = FieldType.Select)]
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置操作对象。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column]
    public string Target { get; set; }

    /// <summary>
    /// 取得或设置操作内容。
    /// </summary>
    [Column(IsQuery = true)]
    public string Content { get; set; }
}