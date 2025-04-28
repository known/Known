namespace Known;

/// <summary>
/// 角色信息类。
/// </summary>
[DisplayName("系统角色")]
public class RoleInfo
{
    /// <summary>
    /// 构造函数。
    /// </summary>
    public RoleInfo()
    {
        Id = Utils.GetNextId();
        Enabled = true;
    }

    /// <summary>
    /// 取得或设置实体ID。
    /// </summary>
    public string Id { get; set; }

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
    /// 取得或设置角色关联的模块列表。
    /// </summary>
    public List<ModuleInfo> Modules { get; set; } = [];

    /// <summary>
    /// 取得或设置角色关联的菜单ID列表。
    /// </summary>
    public List<string> MenuIds { get; set; } = [];
}