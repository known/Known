using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Known.Entities;

/// <summary>
/// 系统角色实体类。
/// </summary>
public class SysRole : EntityBase
{
    public SysRole()
    {
        Enabled = true;
    }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [Column(IsGrid = true, IsQuery = true, IsForm = true, IsViewLink = true)]
    [DisplayName("名称")]
    [Required(ErrorMessage = "名称不能为空！")]
    [MaxLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置状态。
    /// </summary>
    [Column(IsGrid = true, IsForm = true)]
    [DisplayName("状态")]
    [Required(ErrorMessage = "状态不能为空！")]
    public bool Enabled { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column(IsGrid = true, IsForm = true)]
    [DisplayName("备注")]
    [MaxLength(500)]
    public string Note { get; set; }

    public virtual List<MenuItem> Menus { get; set; }
    public virtual string[] MenuIds { get; set; }
}