using System.Collections.Generic;
using Known.Core.Entities;

namespace Known.Core
{
    public interface ISystemRepository
    {
        #region Module
        bool ExistsSubModule(Database db, string parentId);
        PagingResult<SysModule> QueryModules(Database db, PagingCriteria criteria);
        List<SysModule> GetModules(Database db);
        #endregion

        #region Role
        PagingResult<SysRole> QueryRoles(Database db, PagingCriteria criteria);
        List<SysRole> GetRoles(Database db);
        #endregion

        #region User
        PagingResult<SysUser> QueryUsers(Database db, PagingCriteria criteria);
        List<string> GetUserRoles(Database db, string userId);
        void DeleteUserRoles(Database db, string userId);
        void AddUserRole(Database db, string userId, string roleId);
        #endregion
    }

    class SystemRepository : ISystemRepository
    {
        #region Module
        public bool ExistsSubModule(Database db, string parentId)
        {
            var sql = "select count(*) from SysModule where ParentId=@parentId";
            return db.Scalar<int>(sql, new { parentId }) > 0;
        }

        public PagingResult<SysModule> QueryModules(Database db, PagingCriteria criteria)
        {
            var sql = "select * from SysModule";
            return db.QueryPage<SysModule>(sql, criteria);
        }

        public List<SysModule> GetModules(Database db)
        {
            var sql = "select * from SysModule order by Sort";
            return db.QueryList<SysModule>(sql);
        }
        #endregion

        #region Role
        public PagingResult<SysRole> QueryRoles(Database db, PagingCriteria criteria)
        {
            var sql = "select * from SysRole";
            return db.QueryPage<SysRole>(sql, criteria);
        }

        public List<SysRole> GetRoles(Database db)
        {
            var sql = "select * from SysRole order by CreateTime";
            return db.QueryList<SysRole>(sql);
        }
        #endregion

        #region User
        public PagingResult<SysUser> QueryUsers(Database db, PagingCriteria criteria)
        {
            var sql = "select * from SysUser where UserName<>'System'";
            return db.QueryPage<SysUser>(sql, criteria);
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
        #endregion
    }
}
