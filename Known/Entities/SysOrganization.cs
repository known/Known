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
    [Column]
    [DisplayName("上级组织")]
    [MaxLength(50)]
    public string ParentId { get; set; }

    /// <summary>
    /// 取得或设置编码。
    /// </summary>
    [Column(IsGrid = true, IsForm = true, IsViewLink = true)]
    [DisplayName("编码")]
    [Required(ErrorMessage = "编码不能为空！")]
    [MaxLength(50)]
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [Column(IsGrid = true, IsForm = true)]
    [DisplayName("名称")]
    [Required(ErrorMessage = "名称不能为空！")]
    [MaxLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置管理者。
    /// </summary>
    [Column]
    [DisplayName("管理者")]
    [MaxLength(50)]
    public string ManagerId { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column(IsGrid = true, IsForm = true)]
    [DisplayName("备注")]
    [MaxLength(500)]
    public string Note { get; set; }

    [Column("上级组织", IsGrid = true)]
    public virtual string ParentName { get; set; }
    public virtual string FullName => $"{Code}-{Name}";
}