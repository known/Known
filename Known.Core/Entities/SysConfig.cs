namespace Known.Entities;

/// <summary>
/// 系统配置实体类。
/// </summary>
public class SysConfig
{
    /// <summary>
    /// 取得或设置系统ID。
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string AppId { get; set; }

    /// <summary>
    /// 取得或设置配置键。
    /// </summary>
    [Required]
    [MaxLength(250)]
    public string ConfigKey { get; set; }

    /// <summary>
    /// 取得或设置配置值。
    /// </summary>
    public string ConfigValue { get; set; }
}