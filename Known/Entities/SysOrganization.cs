namespace Known.Entities;

/// <summary>
/// 组织架构实体类。
/// </summary>
public class SysOrganization : EntityBase
{
    /// <summary>
    /// 取得或设置上级组织。
    /// </summary>
    [Column("上级组织", "", false, "1", "50", IsGrid = false)]
    public string ParentId { get; set; }

    /// <summary>
    /// 取得或设置编码。
    /// </summary>
    [Column("编码", "", true, "1", "50")]
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [Column("名称", "", true, "1", "50")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置管理者。
    /// </summary>
    [Column("管理者", "", false, "1", "50")]
    public string ManagerId { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column("备注", "", false, "1", "500")]
    public string Note { get; set; }

    [Column("上级组织")] public virtual string ParentName { get; set; }
    public virtual string FullName => $"{Code}-{Name}";
}