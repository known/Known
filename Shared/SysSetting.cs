namespace Known;

/// <summary>
/// 系统设置实体类。
/// </summary>
[DisplayName("系统设置")]
public class SysSetting : EntityBase
{
    /// <summary>
    /// 取得或设置业务类型。
    /// </summary>
    [Required]
    [MaxLength(250)]
    [DisplayName("业务类型")]
    public string BizType { get; set; }

    /// <summary>
    /// 取得或设置业务名称。
    /// </summary>
    [MaxLength(250)]
    [DisplayName("业务名称")]
    public string BizName { get; set; }

    /// <summary>
    /// 取得或设置业务数据。
    /// </summary>
    [DisplayName("业务数据")]
    public string BizData { get; set; }
}