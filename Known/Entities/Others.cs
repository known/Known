namespace Known.Entities;

/// <summary>
/// 系统配置实体类。
/// </summary>
public class SysConfig
{
    /// <summary>
    /// 取得或设置系统ID。
    /// </summary>
    public string AppId { get; set; }

    /// <summary>
    /// 取得或设置配置键。
    /// </summary>
    public string ConfigKey { get; set; }

    /// <summary>
    /// 取得或设置配置值。
    /// </summary>
    public string ConfigValue { get; set; }
}

/// <summary>
/// 系统角色模块实体类。
/// </summary>
public class SysRoleModule
{
    /// <summary>
    /// 取得或设置角色ID。
    /// </summary>
    public string RoleId { get; set; }

    /// <summary>
    /// 取得或设置模块ID。
    /// </summary>
    public string ModuleId { get; set; }
}

/// <summary>
/// 系统用户角色实体类。
/// </summary>
public class SysUserRole
{
    /// <summary>
    /// 取得或设置用户ID。
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// 取得或设置角色ID。
    /// </summary>
    public string RoleId { get; set; }
}