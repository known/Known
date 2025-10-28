namespace Known.Extensions;

static class RoleExtension
{
    internal static Task<List<string>> GetRoleModuleIdsAsync(this Database db, string userId)
    {
        var sql = $@"select a.{db.FormatName(nameof(SysRoleModule.ModuleId))} 
from {db.FormatName(nameof(SysRoleModule))} a 
where a.{db.FormatName(nameof(SysRoleModule.RoleId))} in (
  select {db.FormatName(nameof(SysUserRole.RoleId))} 
  from {db.FormatName(nameof(SysUserRole))} 
  where {db.FormatName(nameof(SysUserRole.UserId))}=@UserId
) and exists (
  select 1 from {db.FormatName(nameof(SysRole))} 
  where {db.FormatName(nameof(SysRole.Id))}=a.{db.FormatName(nameof(SysRoleModule.RoleId))} 
    and {db.FormatName(nameof(SysRole.Enabled))}=@Enabled
)";
        return db.ScalarsAsync<string>(sql, new { UserId = userId, Enabled = db.FormatBoolean(true) });
    }

    internal static void UpdateUserRoleName(this Database db)
    {
        Task.Run(async () =>
        {
            try
            {
                var users = await db.QueryListAsync<SysUser>();
                foreach (var user in users)
                {
                    if (user.UserName.Equals(Constants.SysUserName, StringComparison.CurrentCultureIgnoreCase))
                        continue;

                    var userRoles = await db.QueryListAsync<SysUserRole>(d => d.UserId == user.Id);
                    var roleIds = userRoles?.Select(d => d.RoleId).ToArray();
                    var roles = await db.QueryListByIdAsync<SysRole>(roleIds);
                    if (roles != null && roles.Count > 0)
                        user.Role = string.Join(",", [.. roles.Select(r => r.Name)]);
                    await db.SaveAsync(user);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(LogTarget.BackEnd, db.User, ex);
            }
        });
    }
}