using System.Collections.Generic;
using Known.Web.Entities;

namespace Known.Web
{
    public interface ISystemRepository
    {
        #region Dictionary
        List<SysDictionary> GetCategories(Database db, string compNo);
        PagingResult<SysDictionary> QueryDictionarys(Database db, PagingCriteria criteria);
        #endregion

        #region Log
        PagingResult<SysLog> QueryLogs(Database db, PagingCriteria criteria);
        #endregion

        #region Organization
        PagingResult<SysOrganization> QueryOrganizations(Database db, PagingCriteria criteria);
        List<SysOrganization> GetOrganizations(Database db);
        bool ExistsOrganization(Database db, SysOrganization entity);
        bool ExistsSubOrganization(Database db, string parentId);
        #endregion

        #region Role
        PagingResult<SysRole> QueryRoles(Database db, PagingCriteria criteria);
        void DeleteRoleUsers(Database db, string roleId);
        List<MenuInfo> GetRoleModules(Database db, string roleId);
        void DeleteRoleModules(Database db, string roleId);
        void AddRoleModule(Database db, string roleId, string moduleId);
        #endregion

        #region User
        PagingResult<SysUser> QueryUsers(Database db, PagingCriteria criteria);
        List<SysRole> GetRoles(Database db, string compNo);
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
        #region Dictionary
        public List<SysDictionary> GetCategories(Database db, string compNo)
        {
            var sql = "select * from SysDictionary where Category='KnownDict' and CompNo=@compNo order by Sort";
            return db.QueryList<SysDictionary>(sql, new { compNo });
        }

        public PagingResult<SysDictionary> QueryDictionarys(Database db, PagingCriteria criteria)
        {
            var sql = "select * from SysDictionary where CompNo=@CompNo and Category=@Category";
            if (!string.IsNullOrWhiteSpace((string)criteria.Parameter.key))
            {
                var key = criteria.Parameter.key;
                sql += " and (Code like @key or Name like @key)";
                criteria.Parameter.key = $"%{key}%";
            }

            return db.QueryPage<SysDictionary>(sql, criteria);
        }
        #endregion

        #region Log
        public PagingResult<SysLog> QueryLogs(Database db, PagingCriteria criteria)
        {
            var sql = "select * from SysLog where 1=1";
            if (!string.IsNullOrWhiteSpace((string)criteria.Parameter.key))
            {
                var key = criteria.Parameter.key;
                sql += " and Content like @key";
                criteria.Parameter.key = $"%{key}%";
            }

            return db.QueryPage<SysLog>(sql, criteria);
        }
        #endregion

        #region Organization
        public PagingResult<SysOrganization> QueryOrganizations(Database db, PagingCriteria criteria)
        {
            var sql = "select * from SysOrganization";
            return db.QueryPage<SysOrganization>(sql, criteria);
        }

        public List<SysOrganization> GetOrganizations(Database db)
        {
            var sql = "select * from SysOrganization order by ParentId,Code";
            return db.QueryList<SysOrganization>(sql);
        }

        public bool ExistsOrganization(Database db, SysOrganization entity)
        {
            var sql = "select count(*) from SysOrganization where Id<>@Id and CompNo=@CompNo and Code=@Code";
            return db.Scalar<int>(sql, new { entity.Id, entity.CompNo, entity.Code }) > 0;
        }

        public bool ExistsSubOrganization(Database db, string parentId)
        {
            var sql = "select count(*) from SysOrganization where ParentId=@parentId";
            return db.Scalar<int>(sql, new { parentId }) > 0;
        }
        #endregion

        #region Role
        public PagingResult<SysRole> QueryRoles(Database db, PagingCriteria criteria)
        {
            var sql = "select * from SysRole where 1=1";
            if (!string.IsNullOrWhiteSpace((string)criteria.Parameter.key))
            {
                var key = criteria.Parameter.key;
                sql += " and Name like @key";
                criteria.Parameter.key = $"%{key}%";
            }

            return db.QueryPage<SysRole>(sql, criteria);
        }

        public void DeleteRoleUsers(Database db, string roleId)
        {
            var sql = "delete from SysUserRole where RoleId=@roleId";
            db.Execute(sql, new { roleId });
        }

        public List<MenuInfo> GetRoleModules(Database db, string roleId)
        {
            var sql = @"
select a.*,case when b.ModuleId is not null then 1 else 0 end Checked
from SysModule a
left join (select * from SysRoleModule where RoleId=@roleId) b on b.ModuleId=a.Id 
order by a.Sort";
            return db.QueryList<MenuInfo>(sql, new { roleId });
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
        #endregion
    }
}