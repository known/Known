namespace Known.Data;

public partial class Database
{
    /// <summary>
    /// 异步获取用户角色模块ID列表。
    /// </summary>
    /// <param name="userId">用户ID。</param>
    /// <returns>用户角色模块ID列表。</returns>
    public virtual Task<List<string>> GetRoleModuleIdsAsync(string userId)
    {
        var sql = $@"select a.{FormatName("ModuleId")} from {FormatName("SysRoleModule")} a 
where a.{FormatName("RoleId")} in (select {FormatName("RoleId")} from {FormatName("SysUserRole")} where {FormatName("UserId")}=@UserId)
  and exists (select 1 from {FormatName("SysRole")} where {FormatName("Id")}=a.{FormatName("RoleId")} and {FormatName("Enabled")}='True')";
        return ScalarsAsync<string>(sql, new { UserId = userId });
    }
}