namespace Known.Entities;

/// <summary>
/// 系统用户角色实体类。
/// </summary>
[DisplayName("系统用户角色")]
public class SysUserRole
{
    /// <summary>
    /// 取得或设置用户ID。
    /// </summary>
    [Required, Key]
    [MaxLength(50)]
    [DisplayName("用户ID")]
    public string UserId { get; set; }

    /// <summary>
    /// 取得或设置角色ID。
    /// </summary>
    [Required, Key]
    [MaxLength(50)]
    [DisplayName("角色ID")]
    public string RoleId { get; set; }
}