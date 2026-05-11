namespace Known.Entities;

/// <summary>
/// 系统字段实体类。
/// </summary>
[DisplayName("系统字段")]
public partial class SysField : EntityBase
{
    /// <summary>
    /// 取得或设置字段代码。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [DisplayName("代码")]
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置字段名称。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [DisplayName("名称")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置字段类型。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Category(nameof(FieldType))]
    [DisplayName("类型")]
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置字段长度。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("长度")]
    public string Length { get; set; }

    /// <summary>
    /// 取得或设置字段是否必填。
    /// </summary>
    [DisplayName("必填")]
    public bool Required { get; set; }
}