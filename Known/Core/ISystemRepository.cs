using System.Collections.Generic;
using Known.Core.Entities;

namespace Known.Core
{
    public interface ISystemRepository
    {
        #region Module
        PagingResult<SysModule> QueryModules(Database db, PagingCriteria criteria);
        List<SysModule> GetModules(Database db);
        bool ExistsSubModule(Database db, string parentId);
        void DeleteModuleRights(Database db, string moduleId);
        #endregion

        #region Role
        PagingResult<SysRole> QueryRoles(Database db, PagingCriteria criteria);
        List<SysRole> GetRoles(Database db);
        void DeleteRoleUsers(Database db, string roleId);
        List<string> GetRoleModules(Database db, string roleId);
        void DeleteRoleModules(Database db, string roleId);
        void AddRoleModule(Database db, string roleId, string moduleId);
        #endregion

        #region User
        PagingResult<SysUser> QueryUsers(Database db, PagingCriteria criteria);
        List<string> GetUserRoles(Database db, string userId);
        void DeleteUserRoles(Database db, string userId);
        void AddUserRole(Database db, string userId, string roleId);
        List<string> GetUserModules(Database db, string userId);
        void DeleteUserModules(Database db, string userId);
        void AddUserModule(Database db, string userId, string moduleId);
        #endregion
    }

    class SystemRepository : ISystemRepository
    {
        #region Module
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

        public bool ExistsSubModule(Database db, string parentId)
        {
            var sql = "select count(*) from SysModule where ParentId=@parentId";
            return db.Scalar<int>(sql, new { parentId }) > 0;
        }

        public void DeleteModuleRights(Database db, string moduleId)
        {
            var sql = "delete from SysRoleModule where ModuleId=@moduleId";
            db.Execute(sql, new { moduleId });

            sql = "delete from SysUserModule where ModuleId=@moduleId";
            db.Execute(sql, new { moduleId });
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

        public void DeleteRoleUsers(Database db, string roleId)
        {
            var sql = "delete from SysUserRole where RoleId=@roleId";
            db.Execute(sql, new { roleId });
        }

        public List<string> GetRoleModules(Database db, string roleId)
        {
            var sql = "select ModuleId from SysRoleModule where RoleId=@roleId";
            return db.Scalars<string>(sql, new { roleId });
        }

        public void DeleteRoleModules(Database db, string roleId)
        {
            var sql = "delete from SysRoleModule where RoleId=@roleId";
            db.Execute(sql, new { roleId });

        }

        public void AddRoleModule(Database db, string roleId, string moduleId)
        {
            var sql = "insert into SysRoleModule(RoleId,ModuleId) values(@roleId,@moduleId)";
            db.Execute(sql, new { roleId, moduleId });
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
        #endregion
    }
}
