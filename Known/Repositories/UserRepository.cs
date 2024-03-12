using Known.Entities;

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
        var orgNo = criteria.GetParameter(orgNoId);
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

    internal static async Task<bool> ExistsUserNameAsync(Database db, string id, string userName)
    {
        var sql = "select count(*) from SysUser where Id<>@id and UserName=@userName";
        return await db.ScalarAsync<int>(sql, new { id, userName }) > 0;
    }

    internal static Task<int> GetUserCountAsync(Database db)
    {
        var sql = "select count(*) from SysUser where CompNo=@CompNo";
        return db.ScalarAsync<int>(sql, new { db.User.CompNo });
    }

    internal static Task<List<string>> GetUserRolesAsync(Database db, string userId)
    {
        var sql = "select RoleId from SysUserRole where UserId=@userId";
        return db.ScalarsAsync<string>(sql, new { userId });
    }

    internal static Task<int> DeleteUserRolesAsync(Database db, string userId)
    {
        var sql = "delete from SysUserRole where UserId=@userId";
        return db.ExecuteAsync(sql, new { userId });
    }

    internal static Task<int> AddUserRoleAsync(Database db, string userId, string roleId)
    {
        var sql = "insert into SysUserRole(UserId,RoleId) values(@userId,@roleId)";
        return db.ExecuteAsync(sql, new { userId, roleId });
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
        var sql = "select * from SysUser where UserName=@userName";
        return db.QueryAsync<SysUser>(sql, new { userName });
    }

    internal static Task<UserInfo> GetUserAsync(Database db, string userName)
    {
        var sql = "select * from SysUser where UserName=@userName";
        return db.QueryAsync<UserInfo>(sql, new { userName });
    }

    internal static Task<SysUser> GetUserAsync(Database db, string userName, string password)
    {
        password = Utils.ToMd5(password);
        var sql = "select * from SysUser where UserName=@userName and Password=@password";
        return db.QueryAsync<SysUser>(sql, new { userName, password });
    }

    internal static Task<string> GetOrgNameAsync(Database db, string appId, string compNo, string orgNo)
    {
        var sql = "select Name from SysOrganization where AppId=@appId and CompNo=@compNo and Code=@orgNo";
        return db.ScalarAsync<string>(sql, new { appId, compNo, orgNo });
    }

    //Message
    internal static Task<PagingResult<SysMessage>> QueryMessagesAsync(Database db, PagingCriteria criteria)
    {
        var sql = "select * from SysMessage where CompNo=@CompNo";
        return db.QueryPageAsync<SysMessage>(sql, criteria);
    }

    internal static Task<int> GetMessageCountAsync(Database db)
    {
        var sql = $"select count(*) from SysMessage where UserId=@UserName and Status='{Constants.UMStatusUnread}'";
        return db.ScalarAsync<int>(sql, new { db.User.UserName });
    }
}