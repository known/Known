namespace Known.Entities;

/// <summary>
/// 系统用户角色实体类。
/// </summary>
public class SysUserRole
{
    /// <summary>
    /// 取得或设置用户ID。
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string UserId { get; set; }

    /// <summary>
    /// 取得或设置角色ID。
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string RoleId { get; set; }
}