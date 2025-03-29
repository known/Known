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
}