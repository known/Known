namespace Known.Entities;

/// <summary>
/// 系统角色实体类。
/// </summary>
[DisplayName("系统角色")]
public class SysRole : EntityBase
{
    /// <summary>
    /// 构造函数。
    /// </summary>
    public SysRole()
    {
        Enabled = true;
    }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column(IsQuery = true, IsViewLink = true, Width = 150)]
    [Form]
    [DisplayName("名称")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置状态。
    /// </summary>
    [Required]
    [Column(Width = 80)]
    [Form(Type = nameof(FieldType.Switch))]
    [Category(nameof(StatusType))]
    [DisplayName("状态")]
    public bool Enabled { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column]
    [Form(Type = nameof(FieldType.TextArea))]
    [DisplayName("备注")]
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置角色关联的菜单列表。
    /// </summary>
    public virtual List<MenuInfo> Menus { get; set; } = [];

    /// <summary>
    /// 取得或设置角色关联的菜单ID列表。
    /// </summary>
    public virtual List<string> MenuIds { get; set; } = [];
}