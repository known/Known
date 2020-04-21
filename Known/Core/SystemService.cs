using System.Collections.Generic;
using Known.Core.Entities;

namespace Known.Core
{
    public class SystemService : ServiceBase
    {
        private const string DEFALUT_PWD = "123456";

        private ISystemRepository Repository
        {
            get { return Container.Resolve<ISystemRepository>(); }
        }

        #region Module
        public PagingResult<SysModule> QueryModules(PagingCriteria criteria)
        {
            return Repository.QueryModules(Database, criteria);
        }

        public Result DeleteModules(string[] ids)
        {
            var entities = Database.QueryListById<SysModule>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error("请至少选择一条记录进行操作！");

            foreach (var item in entities)
            {
                if (Repository.ExistsSubModule(Database, item.Id))
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

        public Result CopyModules(string[] ids, string mid)
        {
            var module = Database.QueryById<SysModule>(mid);
            if (module == null)
                return Result.Error("所选模块不存在！");

            var entities = Database.QueryListById<SysModule>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error("请至少选择一条记录进行操作！");

            return Database.Transaction("复制", db =>
            {
                foreach (var item in entities)
                {
                    item.ParentId = module.Id;
                    db.Insert(item);
                }
            });
        }

        public List<SysModule> GetModules()
        {
            return Repository.GetModules(Database);
        }

        public SysModule GetModule(string id)
        {
            return Database.QueryById<SysModule>(id);
        }

        public Result SaveModule(dynamic model)
        {
            var entity = Database.QueryById<SysModule>((string)model.Id);
            if (entity == null)
                entity = new SysModule();

            entity.FillModel(model);
            var vr = entity.Validate();
            if (!vr.IsValid)
                return Result.Error(vr.Message);

            Database.Save(entity);
            return Result.Success("保存成功！", entity.Id);
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
                }
            });
        }

        public List<SysRole> GetRoles()
        {
            var roles = Repository.GetRoles(Database);
            if (roles == null)
                roles = new List<SysRole>();

            return roles;
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
                return Result.Error(vr.Message);

            Database.Save(entity);
            return Result.Success("保存成功！", entity.Id);
        }
        #endregion

        #region User
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
                return Result.Error(vr.Message);

            Database.Save(entity);
            return Result.Success("保存成功！", entity.Id);
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
        #endregion
    }
}
