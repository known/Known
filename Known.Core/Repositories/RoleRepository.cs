namespace Known.Core.Repositories;

class RoleRepository
{
    internal static PagingResult<SysRole> QueryRoles(Database db, PagingCriteria criteria)
    {
        var sql = "select * from SysRole where AppId=@AppId and CompNo=@CompNo";
        return db.QueryPage<SysRole>(sql, criteria);
    }

    internal static List<SysRole> GetRoles(Database db, string appId, string compNo)
    {
        var sql = "select * from SysRole where AppId=@appId and CompNo=@compNo and Enabled='True' order by CreateTime";
        return db.QueryList<SysRole>(sql, new { appId, compNo });
    }

    internal static void DeleteRoleUsers(Database db, string roleId)
    {
        var sql = "delete from SysUserRole where RoleId=@roleId";
        db.Execute(sql, new { roleId });
    }

    internal static List<string> GetRoleModuleIds(Database db, string roleId)
    {
        var sql = @"select ModuleId from SysRoleModule where RoleId=@roleId";
        return db.Scalars<string>(sql, new { roleId });
    }

    internal static void DeleteRoleModules(Database db, string roleId)
    {
        var sql = "delete from SysRoleModule where RoleId=@roleId";
        db.Execute(sql, new { roleId });
    }

    internal static void AddRoleModule(Database db, string roleId, string moduleId)
    {
        var sql = "insert into SysRoleModule(RoleId,ModuleId) values(@roleId,@moduleId)";
        db.Execute(sql, new { roleId, moduleId });
    }
}