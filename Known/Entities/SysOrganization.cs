using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Known.Entities;

/// <summary>
/// 组织架构实体类。
/// </summary>
public class SysOrganization : EntityBase
{
    /// <summary>
    /// 取得或设置上级组织。
    /// </summary>
    [DisplayName("上级组织")]
    [MaxLength(50)]
    public string ParentId { get; set; }

    /// <summary>
    /// 取得或设置编码。
    /// </summary>
    [DisplayName("编码")]
    [Required]
    [MaxLength(50)]
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [DisplayName("名称")]
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置管理者。
    /// </summary>
    [DisplayName("管理者")]
    [MaxLength(50)]
    public string ManagerId { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [DisplayName("备注")]
    [MaxLength(500)]
    public string Note { get; set; }

    [DisplayName("上级组织")]
    public virtual string ParentName { get; set; }
    public virtual string FullName => $"{Code}-{Name}";
}