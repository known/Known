namespace Known.Entities;

/// <summary>
/// 组织架构实体类。
/// </summary>
[DisplayName("组织架构")]
public class SysOrganization : EntityBase
{
    /// <summary>
    /// 取得或设置上级组织。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("上级组织")]
    public string ParentId { get; set; }

    /// <summary>
    /// 取得或设置代码。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column(IsViewLink = true)]
    [Form]
    [DisplayName("编码")]
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column]
    [Form]
    [DisplayName("名称")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置管理者。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("管理者ID")]
    public string ManagerId { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column]
    [Form(Type = nameof(FieldType.TextArea))]
    [DisplayName("备注")]
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置上级组织名称。
    /// </summary>
    public virtual string ParentName { get; set; }
}