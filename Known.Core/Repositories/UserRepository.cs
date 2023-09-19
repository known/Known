namespace Known.Core.Repositories;

class UserRepository
{
    //User
    internal static PagingResult<SysUser> QueryUsers(Database db, PagingCriteria criteria)
    {
        var sql = @"
select a.*,b.Name Department 
from SysUser a 
left join SysOrganization b on b.Id=a.OrgNo 
where a.AppId=@AppId and a.CompNo=@CompNo and a.UserName<>'admin' and a.UserName<>a.CompNo";
        if (criteria.HasQuery("OrgNo"))
        {
            var orgNo = criteria.GetQueryValue("OrgNo");
            if (orgNo != db.User.CompNo)
                sql += " and a.OrgNo=@OrgNo";
        }
        return db.QueryPage<SysUser>(sql, criteria);
    }

    internal static bool ExistsUserName(Database db, string id, string userName)
    {
        var sql = "select count(*) from SysUser where Id<>@id and UserName=@userName";
        return db.Scalar<int>(sql, new { id, userName }) > 0;
    }

    internal static int GetUserCount(Database db)
    {
        var sql = "select count(*) from SysUser where CompNo=@CompNo";
        return db.Scalar<int>(sql, new { db.User.CompNo });
    }

    internal static List<string> GetUserRoles(Database db, string userId)
    {
        var sql = "select RoleId from SysUserRole where UserId=@userId";
        return db.Scalars<string>(sql, new { userId });
    }

    internal static void DeleteUserRoles(Database db, string userId)
    {
        var sql = "delete from SysUserRole where UserId=@userId";
        db.Execute(sql, new { userId });
    }

    internal static void AddUserRole(Database db, string userId, string roleId)
    {
        var sql = "insert into SysUserRole(UserId,RoleId) values(@userId,@roleId)";
        db.Execute(sql, new { userId, roleId });
    }

    //Account
    internal static List<string> GetUserModuleIds(Database db, string userId)
    {
        var sql = @"select a.ModuleId from SysRoleModule a 
where a.RoleId in (select RoleId from SysUserRole where UserId=@userId)
  and exists (select 1 from SysRole where Id=a.RoleId and Enabled='True')";
        return db.Scalars<string>(sql, new { userId });
    }

    internal static SysUser GetUserByUserName(Database db, string userName)
    {
        var sql = "select * from SysUser where UserName=@userName";
        return db.Query<SysUser>(sql, new { userName });
    }

    internal static UserInfo GetUser(Database db, string userName)
    {
        var sql = "select * from SysUser where UserName=@userName";
        return db.Query<UserInfo>(sql, new { userName });
    }

    internal static SysUser GetUser(Database db, string userName, string password)
    {
        password = Utils.ToMd5(password);
        var sql = "select * from SysUser where UserName=@userName and Password=@password";
        return db.Query<SysUser>(sql, new { userName, password });
    }

    internal static string GetOrgName(Database db, string appId, string compNo, string orgNo)
    {
        var sql = "select Name from SysOrganization where AppId=@appId and CompNo=@compNo and Code=@orgNo";
        return db.Scalar<string>(sql, new { appId, compNo, orgNo });
    }

    //Message
    internal static PagingResult<SysMessage> QueryMessages(Database db, PagingCriteria criteria)
    {
        var sql = "select * from SysMessage where CompNo=@CompNo";
        return db.QueryPage<SysMessage>(sql, criteria);
    }

    internal static int GetMessageCount(Database db)
    {
        var sql = $"select count(*) from SysMessage where UserId=@UserName and Status='{Constants.UMStatusUnread}'";
        return db.Scalar<int>(sql, new { db.User.UserName });
    }
}