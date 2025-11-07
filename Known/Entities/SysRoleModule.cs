namespace Known.Entities;

/// <summary>
/// 系统角色模块实体类。
/// </summary>
[DisplayName("系统角色模块")]
public class SysRoleModule
{
    /// <summary>
    /// 取得或设置角色ID。
    /// </summary>
    [Required, Key]
    [MaxLength(50)]
    [DisplayName("角色ID")]
    public string RoleId { get; set; }

    /// <summary>
    /// 取得或设置模块ID。
    /// </summary>
    [Required, Key]
    [MaxLength(250)]
    [DisplayName("模块ID")]
    public string ModuleId { get; set; }
}