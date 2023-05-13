namespace Known.Entities;

/// <summary>
/// 系统日志实体类。
/// </summary>
public class SysLog : EntityBase
{
    /// <summary>
    /// 取得或设置操作类型。
    /// </summary>
    [Column("操作类型", "", true, "1", "50")]
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置操作对象。
    /// </summary>
    [Column("操作对象", "", true, "1", "50")]
    public string Target { get; set; }

    /// <summary>
    /// 取得或设置操作内容。
    /// </summary>
    [Column("操作内容", "")]
    public string Content { get; set; }
}