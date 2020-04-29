using System.Collections.Generic;
using Known.Core.Entities;

namespace Known.Core.Repositories
{
    public interface IRoleRepository
    {
        PagingResult<SysRole> QueryRoles(Database db, PagingCriteria criteria);
        void DeleteRoleUsers(Database db, string roleId);
        List<MenuInfo> GetRoleModules(Database db, string roleId);
        void DeleteRoleModules(Database db, string roleId);
        void AddRoleModule(Database db, string roleId, string moduleId);
    }

    class RoleRepository : IRoleRepository
    {
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
left join (select * from SysRoleModule where RoleId=@roleId) b on b.ModuleId=a.Id";
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
    }
}
