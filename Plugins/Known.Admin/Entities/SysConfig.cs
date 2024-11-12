namespace Known.Admin.Entities;

/// <summary>
/// 系统配置实体类。
/// </summary>
public class SysConfig
{
    /// <summary>
    /// 取得或设置系统ID。
    /// </summary>
    public string AppId { get; set; }

    /// <summary>
    /// 取得或设置配置键。
    /// </summary>
    public string ConfigKey { get; set; }

    /// <summary>
    /// 取得或设置配置值。
    /// </summary>
    public string ConfigValue { get; set; }
}