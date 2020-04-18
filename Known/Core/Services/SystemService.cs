using System.Collections.Generic;
using Known.Core.Datas;
using Known.Core.Entities;

namespace Known.Core.Services
{
    class SystemService : ServiceBase
    {
        private const string DEFALUT_PWD = "123456";

        private ISystemRepository Repository
        {
            get { return Container.Resolve<ISystemRepository>(); }
        }

        #region Module
        internal PagingResult<SysModule> QueryModules(PagingCriteria criteria)
        {
            return Repository.QueryModules(Database, criteria);
        }

        internal Result DeleteModules(string[] ids)
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

        internal List<SysModule> GetModules()
        {
            return Repository.GetModules(Database);
        }

        internal SysModule GetModule(string id)
        {
            return Database.QueryById<SysModule>(id);
        }

        internal Result SaveModule(dynamic model)
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
        internal PagingResult<SysRole> QueryRoles(PagingCriteria criteria)
        {
            return Repository.QueryRoles(Database, criteria);
        }

        internal Result DeleteRoles(string[] ids)
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

        internal List<SysRole> GetRoles()
        {
            var roles = Repository.GetRoles(Database);
            if (roles == null)
                roles = new List<SysRole>();

            return roles;
        }

        internal SysRole GetRole(string id)
        {
            return Database.QueryById<SysRole>(id);
        }

        internal Result SaveRole(dynamic model)
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
        internal PagingResult<SysUser> QueryUsers(PagingCriteria criteria)
        {
            return Repository.QueryUsers(Database, criteria);
        }

        internal Result DeleteUsers(string[] ids)
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

        internal Result SetUserPwds(string[] ids)
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

        internal Result EnableUsers(string[] ids, int enable)
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

        internal SysUser GetUser(string id)
        {
            return Database.QueryById<SysUser>(id);
        }

        internal Result SaveUser(dynamic model)
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

        internal string[] GetUserRoles(string userId)
        {
            return new string[] { "f000509503d348068bce1fca93e534bd" };
        }
        #endregion
    }
}
