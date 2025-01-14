namespace Known.Entities;

/// <summary>
/// 系统角色模块实体类。
/// </summary>
public class SysRoleModule
{
    /// <summary>
    /// 取得或设置角色ID。
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string RoleId { get; set; }

    /// <summary>
    /// 取得或设置模块ID。
    /// </summary>
    [Required]
    [MaxLength(250)]
    public string ModuleId { get; set; }
}