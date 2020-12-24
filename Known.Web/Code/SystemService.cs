using System.Collections.Generic;
using Known.Web.Entities;

namespace Known.Web
{
    class SystemService : ServiceBase
    {
        private ISystemRepository Repository => Container.Resolve<ISystemRepository>();

        #region Dictionary
        public List<SysDictionary> GetCategories()
        {
            var compNo = CurrentUser.CompNo;
            return Repository.GetCategories(Database, compNo);
        }

        public PagingResult<SysDictionary> QueryDictionarys(PagingCriteria criteria)
        {
            criteria.Parameter.CompNo = CurrentUser.CompNo;
            return Repository.QueryDictionarys(Database, criteria);
        }

        public Result DeleteDictionarys(string[] ids)
        {
            var entities = Database.QueryListById<SysDictionary>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error("请至少选择一条记录进行操作！");

            return Database.Transaction("删除", db =>
            {
                foreach (var item in entities)
                {
                    db.Delete(item);
                }
            });
        }

        public SysDictionary GetDictionary(string id)
        {
            return Database.QueryById<SysDictionary>(id);
        }

        public Result SaveDictionary(dynamic model)
        {
            var entity = Database.QueryById<SysDictionary>((string)model.Id);
            if (entity == null)
                entity = new SysDictionary();

            entity.FillModel(model);
            var vr = entity.Validate();
            if (!vr.IsValid)
                return vr;

            Database.Save(entity);
            return Result.Success("保存成功！", entity.Id);
        }
        #endregion

        #region Log
        public PagingResult<SysLog> QueryLogs(PagingCriteria criteria)
        {
            return Repository.QueryLogs(Database, criteria);
        }

        public Result DeleteLogs(string[] ids)
        {
            var entities = Database.QueryListById<SysLog>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error("请至少选择一条记录进行操作！");

            return Database.Transaction("删除", db =>
            {
                foreach (var item in entities)
                {
                    db.Delete(item);
                }
            });
        }
        #endregion

        #region Organization
        public PagingResult<SysOrganization> QueryOrganizations(PagingCriteria criteria)
        {
            return Repository.QueryOrganizations(Database, criteria);
        }

        public Result DeleteOrganizations(string[] ids)
        {
            var entities = Database.QueryListById<SysOrganization>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error("请至少选择一条记录进行操作！");

            foreach (var item in entities)
            {
                if (Repository.ExistsSubOrganization(Database, item.Id))
                    return Result.Error($"{item.Name}存在子模块，不能删除！");
            }

            return Database.Transaction("删除", db =>
            {
                foreach (var item in entities)
                {
                    db.Delete(item);
                }
            });
        }

        public List<SysOrganization> GetOrganizations()
        {
            var datas = Repository.GetOrganizations(Database);
            if (datas == null)
                datas = new List<SysOrganization>();

            var app = Config.App;
            datas.Insert(0, new SysOrganization
            {
                Id = app.CompNo,
                ParentId = "",
                Code = app.CompNo,
                Name = app.CompName
            });
            return datas;
        }

        public SysOrganization GetOrganization(string id)
        {
            return Database.QueryById<SysOrganization>(id);
        }

        public Result SaveOrganization(dynamic model)
        {
            var entity = Database.QueryById<SysOrganization>((string)model.Id);
            if (entity == null)
                entity = new SysOrganization();

            entity.FillModel(model);
            var vr = ValidateOrganization(Database, entity);
            if (!vr.IsValid)
                return vr;

            Database.Save(entity);
            return Result.Success("保存成功！", entity.Id);
        }

        private Result ValidateOrganization(Database db, SysOrganization entity)
        {
            var vr = entity.Validate();
            if (vr.IsValid)
            {
                if (Repository.ExistsOrganization(db, entity))
                    vr.AddError("组织编码已存在！");
            }

            return vr;
        }
        #endregion

        #region Role
        public PagingResult<SysRole> QueryRoles(PagingCriteria criteria)
        {
            return Repository.QueryRoles(Database, criteria);
        }

        public Result DeleteRoles(string[] ids)
        {
            var entities = Database.QueryListById<SysRole>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error("请至少选择一条记录进行操作！");

            return Database.Transaction("删除", db =>
            {
                foreach (var item in entities)
                {
                    db.Delete(item);
                    Repository.DeleteRoleUsers(db, item.Id);
                    Repository.DeleteRoleModules(db, item.Id);
                }
            });
        }

        public SysRole GetRole(string id)
        {
            return Database.QueryById<SysRole>(id);
        }

        public Result SaveRole(dynamic model)
        {
            var entity = Database.QueryById<SysRole>((string)model.Id);
            if (entity == null)
                entity = new SysRole();

            entity.FillModel(model);
            var vr = entity.Validate();
            if (!vr.IsValid)
                return vr;

            Database.Save(entity);
            return Result.Success("保存成功！", entity.Id);
        }

        public List<MenuInfo> GetRoleModules(string roleId)
        {
            return Repository.GetRoleModules(Database, roleId);
        }

        public Result SaveRoleModules(string roleId, List<string> moduleIds)
        {
            return Database.Transaction("保存", db =>
            {
                Repository.DeleteRoleModules(db, roleId);
                if (moduleIds != null && moduleIds.Count > 0)
                {
                    foreach (var item in moduleIds)
                    {
                        Repository.AddRoleModule(db, roleId, item);
                    }
                }
            });
        }
        #endregion

        #region User
        private const string DEFALUT_PWD = "123456";

        public PagingResult<SysUser> QueryUsers(PagingCriteria criteria)
        {
            return Repository.QueryUsers(Database, criteria);
        }

        public Result DeleteUsers(string[] ids)
        {
            var entities = Database.QueryListById<SysUser>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error("请至少选择一条记录进行操作！");

            return Database.Transaction("删除", db =>
            {
                foreach (var item in entities)
                {
                    db.Delete(item);
                    Repository.DeleteUserRoles(db, item.Id);
                    Repository.DeleteUserModules(db, item.Id);
                }
            });
        }

        public Result SetUserPwds(string[] ids)
        {
            var entities = Database.QueryListById<SysUser>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error("请至少选择一条记录进行操作！");

            return Database.Transaction("重置", db =>
            {
                foreach (var item in entities)
                {
                    item.Password = Utils.ToMd5(DEFALUT_PWD);
                    db.Save(item);
                }
            });
        }

        public Result EnableUsers(string[] ids, int enable)
        {
            var entities = Database.QueryListById<SysUser>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error("请至少选择一条记录进行操作！");

            var name = enable == 1 ? "启用" : "停用";
            return Database.Transaction(name, db =>
            {
                foreach (var item in entities)
                {
                    item.Enabled = enable;
                    db.Save(item);
                }
            });
        }

        public SysUser GetUser(string id)
        {
            return Database.QueryById<SysUser>(id);
        }

        public Result SaveUser(dynamic model)
        {
            var entity = Database.QueryById<SysUser>((string)model.Id);
            if (entity == null)
            {
                entity = new SysUser();
                entity.Password = Utils.ToMd5(DEFALUT_PWD);
            }

            entity.FillModel(model);
            var vr = entity.Validate();
            if (!vr.IsValid)
                return vr;

            Database.Save(entity);
            return Result.Success("保存成功！", entity.Id);
        }

        public List<SysRole> GetRoles()
        {
            var roles = Repository.GetRoles(Database, "");
            if (roles == null)
                roles = new List<SysRole>();

            return roles;
        }

        public List<string> GetUserRoles(string userId)
        {
            return Repository.GetUserRoles(Database, userId);
        }

        public Result SaveUserRoles(string userId, List<string> roleIds)
        {
            return Database.Transaction("保存", db =>
            {
                Repository.DeleteUserRoles(db, userId);
                if (roleIds != null && roleIds.Count > 0)
                {
                    foreach (var item in roleIds)
                    {
                        Repository.AddUserRole(db, userId, item);
                    }
                }
            });
        }

        public List<string> GetUserModules(string userId)
        {
            return Repository.GetUserModules(Database, userId);
        }

        public Result SaveUserModules(string userId, List<string> moduleIds)
        {
            return Database.Transaction("保存", db =>
            {
                Repository.DeleteUserModules(db, userId);
                if (moduleIds != null && moduleIds.Count > 0)
                {
                    foreach (var item in moduleIds)
                    {
                        Repository.AddUserModule(db, userId, item);
                    }
                }
            });
        }
        #endregion
    }
}