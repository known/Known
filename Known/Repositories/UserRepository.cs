namespace Known.Repositories;

class UserRepository
{
    //User
    internal static async Task<PagingResult<SysUser>> QueryUsersAsync(Database db, PagingCriteria criteria)
    {
        var sql = @"
select a.*,b.Name as Department 
from SysUser a 
left join SysOrganization b on b.Id=a.OrgNo 
where a.AppId=@AppId and a.CompNo=@CompNo and a.UserName<>'admin'";
        var orgNoId = nameof(SysUser.OrgNo);
        var orgNo = criteria.GetParameter<string>(orgNoId);
        if (!string.IsNullOrWhiteSpace(orgNo))
        {
            var org = await db.QueryByIdAsync<SysOrganization>(orgNo);
            if (org != null && org.Code != db.User.CompNo)
                criteria.SetQuery(orgNoId, QueryType.Equal, orgNo);
            else
                criteria.RemoveQuery(orgNoId);
        }
        criteria.Fields[nameof(SysUser.Name)] = "a.Name";
        return await db.QueryPageAsync<SysUser>(sql, criteria);
    }

    internal static Task<List<SysUser>> GetUsersByRoleAsync(Database db, string roleName)
    {
        var sql = $"select * from SysUser where Role like '%{roleName}%'";
        return db.QueryListAsync<SysUser>(sql);
    }

    internal static Task<bool> ExistsUserNameAsync(Database db, string id, string userName)
    {
        return db.ExistsAsync<SysUser>(d => d.Id != id && d.UserName == userName);
    }

    internal static async Task<List<string>> GetUserRolesAsync(Database db, string userId)
    {
        var roles = await db.QueryListAsync<SysUserRole>(d => d.UserId == userId);
        return roles.Select(r => r.RoleId).ToList();
    }

    internal static Task<int> DeleteUserRolesAsync(Database db, string userId)
    {
        return db.DeleteAsync<SysUserRole>(d => d.UserId == userId);
    }

    internal static Task<int> AddUserRoleAsync(Database db, string userId, string roleId)
    {
        return db.InsertDataAsync(new SysUserRole { UserId = userId, RoleId = roleId });
    }

    //Account
    internal static Task<List<string>> GetUserModuleIdsAsync(Database db, string userId)
    {
        var sql = @"select a.ModuleId from SysRoleModule a 
where a.RoleId in (select RoleId from SysUserRole where UserId=@userId)
  and exists (select 1 from SysRole where Id=a.RoleId and Enabled='True')";
        return db.ScalarsAsync<string>(sql, new { userId });
    }

    internal static Task<SysUser> GetUserByUserNameAsync(Database db, string userName)
    {
        return db.QueryAsync<SysUser>(d => d.UserName == userName);
    }

    internal static Task<UserInfo> GetUserInfoByIdAsync(Database db, string id)
    {
        return db.QueryAsync<UserInfo>(d => d.Id == id);
    }

    internal static Task<UserInfo> GetUserInfoAsync(Database db, string userName)
    {
        return db.QueryAsync<UserInfo>(d => d.UserName == userName);
    }

    internal static Task<SysUser> GetUserAsync(Database db, string userName, string password)
    {
        password = Utils.ToMd5(password);
        return db.QueryAsync<SysUser>(d => d.UserName == userName && d.Password == password);
    }
}