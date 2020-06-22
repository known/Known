using System.Collections.Generic;
using Known.Core.Entities;

namespace Known.Core.Repositories
{
    public interface IUserRepository
    {
        PagingResult<SysUser> QueryUsers(Database db, PagingCriteria criteria);
        List<SysRole> GetRoles(Database db, string compNo);
        List<string> GetUserRoles(Database db, string userId);
        void DeleteUserRoles(Database db, string userId);
        void AddUserRole(Database db, string userId, string roleId);
        List<string> GetUserModules(Database db, string userId);
        void DeleteUserModules(Database db, string userId);
        void AddUserModule(Database db, string userId, string moduleId);
    }

    class UserRepository : IUserRepository
    {
        public PagingResult<SysUser> QueryUsers(Database db, PagingCriteria criteria)
        {
            var sql = "select * from SysUser where UserName<>'System' and OrgNo=@OrgNo";
            if (!string.IsNullOrWhiteSpace((string)criteria.Parameter.key))
            {
                var key = criteria.Parameter.key;
                sql += " and (UserName like @key or Name like @key)";
                criteria.Parameter.key = $"%{key}%";
            }

            return db.QueryPage<SysUser>(sql, criteria);
        }

        public List<SysRole> GetRoles(Database db, string compNo)
        {
            var sql = "select * from SysRole order by CreateTime";
            return db.QueryList<SysRole>(sql);
        }

        public List<string> GetUserRoles(Database db, string userId)
        {
            var sql = "select RoleId from SysUserRole where UserId=@userId";
            return db.Scalars<string>(sql, new { userId });
        }

        public void DeleteUserRoles(Database db, string userId)
        {
            var sql = "delete from SysUserRole where UserId=@userId";
            db.Execute(sql, new { userId });
        }

        public void AddUserRole(Database db, string userId, string roleId)
        {
            var sql = "insert into SysUserRole(UserId,RoleId) values(@userId,@roleId)";
            db.Execute(sql, new { userId, roleId });
        }

        public List<string> GetUserModules(Database db, string userId)
        {
            var sql = "select ModuleId from SysUserModule where UserId=@userId";
            return db.Scalars<string>(sql, new { userId });
        }

        public void DeleteUserModules(Database db, string userId)
        {
            var sql = "delete from SysUserModule where UserId=@userId";
            db.Execute(sql, new { userId });
        }

        public void AddUserModule(Database db, string userId, string moduleId)
        {
            var sql = "insert into SysUserModule(UserId,ModuleId) values(@userId,@moduleId)";
            db.Execute(sql, new { userId, moduleId });
        }
    }
}
