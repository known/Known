namespace Known.Entities;

public class SysConfig
{
    public string AppId { get; set; }
    public string ConfigKey { get; set; }
    public string ConfigValue { get; set; }
}

public class SysRoleModule
{
    public string RoleId { get; set; }
    public string ModuleId { get; set; }
}

public class SysUserRole
{
    public string UserId { get; set; }
    public string RoleId { get; set; }
}