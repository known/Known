namespace Known.Extensions;

static class RoleExtension
{
    internal static Task<List<string>> GetRoleModuleIdsAsync(this Database db, string userId)
    {
        var sql = $@"select a.{db.FormatName("ModuleId")} from {db.FormatName("SysRoleModule")} a 
where a.{db.FormatName("RoleId")} in (select {db.FormatName("RoleId")} from {db.FormatName("SysUserRole")} where {db.FormatName("UserId")}=@UserId)
  and exists (select 1 from {db.FormatName("SysRole")} where {db.FormatName("Id")}=a.{db.FormatName("RoleId")} and {db.FormatName("Enabled")}='True')";
        return db.ScalarsAsync<string>(sql, new { UserId = userId });
    }
}