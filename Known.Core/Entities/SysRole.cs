namespace Known.Entities;

/// <summary>
/// 系统角色实体类。
/// </summary>
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
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置状态。
    /// </summary>
    [Required]
    public bool Enabled { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    public string Note { get; set; }
}