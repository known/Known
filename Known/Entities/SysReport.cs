namespace Known.Entities;

/// <summary>
/// 系统报表实体类。
/// </summary>
[DisplayName("系统报表")]
public class SysReport : EntityBase
{
    /// <summary>
    /// 取得或设置子系统ID。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("子系统ID")]
    public string SysId { get; set; }

    /// <summary>
    /// 取得或设置是否是系统固定的，不能删除的报表。
    /// </summary>
    [DisplayName("是否固定")]
    public bool IsFixed { get; set; }

    /// <summary>
    /// 取得或设置报表名称。
    /// </summary>
    [Required]
    [MaxLength(250)]
    [DisplayName("报表名称")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置报表类型。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [DisplayName("报表类型")]
    public string Type { get; set; } = "Table";

    /// <summary>
    /// 取得或设置报表配置，JSON格式字符串。
    /// </summary>
    [DisplayName("配置信息")]
    public string Config { get; set; }

    /// <summary>
    /// 取得或设置备注信息。
    /// </summary>
    [DisplayName("备注")]
    public string Note { get; set; }
}