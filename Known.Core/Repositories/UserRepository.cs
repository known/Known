﻿namespace Known.Core.Repositories;

class UserRepository
{
    internal static PagingResult<SysUser> QueryUsers(Database db, PagingCriteria criteria)
    {
        var sql = "select * from SysUser where AppId=@AppId and CompNo=@CompNo";
        if (criteria.HasParameter("OrgNo"))
        {
            var orgNo = criteria.GetValue("OrgNo");
            if (orgNo != db.User.CompNo)
                sql += " and OrgNo=@OrgNo";
        }
        return db.QueryPage<SysUser>(sql, criteria);
    }

    internal static bool ExistsUserName(Database db, string id, string userName)
    {
        var sql = "select count(*) from SysUser where Id<>@id and UserName=@userName";
        return db.Scalar<int>(sql, new { id, userName }) > 0;
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

    internal static List<string> GetUserModuleIds(Database db, string userId)
    {
        var sql = "select ModuleId from SysRoleModule where RoleId in (select RoleId from SysUserRole where UserId=@userId)";
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
}