namespace Known.Entities;

/// <summary>
/// 通用编码规则实体类。
/// </summary>
[DisplayName("编码规则")]
public class SysNoRule : EntityBase
{
    /// <summary>
    /// 取得或设置代码。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column(Width = 120)]
    [DisplayName("代码")]
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [Required]
    [MaxLength(100)]
    [Column(Width = 180, IsQuery = true)]
    [DisplayName("名称")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置示例。
    /// </summary>
    [MaxLength(100)]
    [Column(Width = 180)]
    [DisplayName("示例")]
    public string Sample { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column]
    [DisplayName("备注")]
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置规则定义。
    /// </summary>
    [DisplayName("规则")]
    public List<NoRuleItem> Rules { get; set; } = [];
}

/// <summary>
/// 编码规则项目类型枚举。
/// </summary>
public enum NoRuleType
{
    /// <summary>
    /// 固定值。
    /// </summary>
    Fixed = 0,
    /// <summary>
    /// 日期时间。
    /// </summary>
    DateTime = 1,
    /// <summary>
    /// 序列号。
    /// </summary>
    Serial = 2
}

/// <summary>
/// 编码规则配置项目类。
/// </summary>
public class NoRuleItem
{
    /// <summary>
    /// 取得或设置规则项目类型。
    /// </summary>
    public NoRuleType Type { get; set; }

    /// <summary>
    /// 取得或设置规则项目值。
    /// </summary>
    public string Value { get; set; }
}