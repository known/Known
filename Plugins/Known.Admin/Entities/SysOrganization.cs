namespace Known.Admin.Entities;

/// <summary>
/// 组织架构实体类。
/// </summary>
public class SysOrganization : EntityBase
{
    /// <summary>
    /// 取得或设置上级组织。
    /// </summary>
    [MaxLength(50)]
    public string ParentId { get; set; }

    /// <summary>
    /// 取得或设置编码。
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置管理者。
    /// </summary>
    [MaxLength(50)]
    public string ManagerId { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [MaxLength(500)]
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置上级组织名称。
    /// </summary>
    public virtual string ParentName { get; set; }
}