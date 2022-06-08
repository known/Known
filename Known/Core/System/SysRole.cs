/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * ------------------------------------------------------------------------------- */

using System.Collections.Generic;
using System.Linq;
using Known.Entities;

namespace Known.Core
{
    partial class SystemService
    {
        public PagingResult<SysRole> QueryRoles(PagingCriteria criteria)
        {
            SetCriteriaAppId(criteria);
            return Repository.QueryRoles(Database, criteria);
        }

        public Result DeleteRoles(string data)
        {
            var ids = Utils.FromJson<string[]>(data);
            var entities = Database.QueryListById<SysRole>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error(Language.SelectOneAtLeast);

            return Database.Transaction(Language.Delete, db =>
            {
                foreach (var item in entities)
                {
                    db.Delete(item);
                    Repository.DeleteRoleUsers(db, item.Id);
                    Repository.DeleteRoleModules(db, item.Id);
                }
            });
        }

        public Result SaveRole(string data, string ids)
        {
            var model = Utils.ToDynamic(data);
            var moduleIds = Utils.FromJson<List<string>>(ids);
            var entity = Database.QueryById<SysRole>((string)model.Id);
            if (entity == null)
                entity = new SysRole();

            entity.FillModel(model);
            var vr = entity.Validate();
            if (!vr.IsValid)
                return vr;

            return Database.Transaction(Language.Save, db =>
            {
                db.Save(entity);
                Repository.DeleteRoleModules(db, entity.Id);
                if (moduleIds != null && moduleIds.Count > 0)
                {
                    foreach (var item in moduleIds)
                    {
                        Repository.AddRoleModule(db, entity.Id, item);
                    }
                }
            }, entity.Id);
        }

        public IEnumerable<object> GetRoleModules(string roleId)
        {
            return Repository.GetRoleModules(Database, CurrentUser.AppId, roleId)
                             .Select(m => m.ToTree());
        }
    }

    partial interface ISystemRepository
    {
        PagingResult<SysRole> QueryRoles(Database db, PagingCriteria criteria);
        void DeleteRoleUsers(Database db, string roleId);
        List<MenuInfo> GetRoleModules(Database db, string appId, string roleId);
        void DeleteRoleModules(Database db, string roleId);
        void AddRoleModule(Database db, string roleId, string moduleId);
    }

    partial class SystemRepository
    {
        public PagingResult<SysRole> QueryRoles(Database db, PagingCriteria criteria)
        {
            var sql = "select * from SysRole where CompNo=@CompNo and AppId=@AppId";
            db.SetQuery(ref sql, criteria, QueryType.Contain, "Name");
            return db.QueryPage<SysRole>(sql, criteria);
        }

        public void DeleteRoleUsers(Database db, string roleId)
        {
            var sql = "delete from SysUserRole where RoleId=@roleId";
            db.Execute(sql, new { roleId });
        }

        public List<MenuInfo> GetRoleModules(Database db, string appId, string roleId)
        {
            if (Config.HasMenu)
            {
                var sql1 = @"select ModuleId from SysRoleModule where RoleId=@roleId";
                var ids = db.Scalars<string>(sql1, new { roleId });
                var menus = new List<MenuInfo>();
                foreach (var item in Config.Menus.Where(m => m.AppId == appId))
                {
                    var menu = Utils.MapTo<MenuInfo>(item);
                    menu.Checked = ids.Contains(menu.Id);
                    menus.Add(menu);
                }
                return menus;
            }

            var sql = @"
select a.*,case when b.ModuleId is not null then 1 else 0 end Checked
from SysModule a
left join (select * from SysRoleModule where RoleId=@roleId) b on b.ModuleId=a.Id 
where a.Enabled=1 and a.AppId=@appId
order by a.Sort";
            return db.QueryList<MenuInfo>(sql, new { roleId, appId });
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