namespace Known.Entities;

/// <summary>
/// 系统设置实体类。
/// </summary>
public class SysSetting : EntityBase
{
    /// <summary>
    /// 取得或设置业务类型。
    /// </summary>
    [Required]
    [MaxLength(250)]
    public string BizType { get; set; }

    /// <summary>
    /// 取得或设置业务名称。
    /// </summary>
    [MaxLength(250)]
    public string BizName { get; set; }

    /// <summary>
    /// 取得或设置业务数据。
    /// </summary>
    public string BizData { get; set; }
}