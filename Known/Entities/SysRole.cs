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
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置状态。
    /// </summary>
    [Required]
    public bool Enabled { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [MaxLength(500)]
    public string Note { get; set; }

    public virtual List<SysModule> Modules { get; set; }
    public virtual List<string> MenuIds { get; set; }
}