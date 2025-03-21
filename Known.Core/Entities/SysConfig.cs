namespace Known.Entities;

/// <summary>
/// 系统配置实体类。
/// </summary>
[DisplayName("系统配置")]
public class SysConfig
{
    /// <summary>
    /// 取得或设置系统ID。
    /// </summary>
    [Required, Key]
    [MaxLength(50)]
    [DisplayName("系统ID")]
    public string AppId { get; set; }

    /// <summary>
    /// 取得或设置配置键。
    /// </summary>
    [Required, Key]
    [MaxLength(250)]
    [DisplayName("配置键")]
    public string ConfigKey { get; set; }

    /// <summary>
    /// 取得或设置配置值。
    /// </summary>
    [DisplayName("配置值")]
    public string ConfigValue { get; set; }
}