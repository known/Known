namespace Known.Repositories;

class RoleRepository
{
    internal static Task<PagingResult<SysRole>> QueryRolesAsync(Database db, PagingCriteria criteria)
    {
        var sql = "select * from SysRole where AppId=@AppId and CompNo=@CompNo";
        return db.QueryPageAsync<SysRole>(sql, criteria);
    }

    internal static Task<List<SysRole>> GetRolesAsync(Database db)
    {
        var sql = "select * from SysRole where AppId=@AppId and CompNo=@CompNo and Enabled='True' order by CreateTime";
        return db.QueryListAsync<SysRole>(sql, new { db.User.AppId, db.User.CompNo });
    }

    internal static Task<SysRole> GetRoleByNameAsync(Database db, string name)
    {
        var sql = "select * from SysRole where AppId=@AppId and CompNo=@CompNo and Name=@name";
        return db.QueryAsync<SysRole>(sql, new { db.User.AppId, db.User.CompNo, name });
    }

    internal static Task<int> DeleteRoleUsersAsync(Database db, string roleId)
    {
        var sql = "delete from SysUserRole where RoleId=@roleId";
        return db.ExecuteAsync(sql, new { roleId });
    }

    internal static Task<List<string>> GetRoleModuleIdsAsync(Database db, string roleId)
    {
        var sql = @"select ModuleId from SysRoleModule where RoleId=@roleId";
        return db.ScalarsAsync<string>(sql, new { roleId });
    }

    internal static Task<int> DeleteRoleModulesAsync(Database db, string roleId)
    {
        var sql = "delete from SysRoleModule where RoleId=@roleId";
        return db.ExecuteAsync(sql, new { roleId });
    }

    internal static Task<int> AddRoleModuleAsync(Database db, string roleId, string moduleId)
    {
        var sql = "insert into SysRoleModule(RoleId,ModuleId) values(@roleId,@moduleId)";
        return db.ExecuteAsync(sql, new { roleId, moduleId });
    }
}