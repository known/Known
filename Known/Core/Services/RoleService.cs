using System.Collections.Generic;
using Known.Core.Entities;
using Known.Core.Repositories;

namespace Known.Core.Services
{
    public class RoleService : ServiceBase
    {
        private IRoleRepository Repository { get; } = Container.Resolve<IRoleRepository>();

        #region View
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
        #endregion

        #region Form
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
        #endregion

        #region RoleModule
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
    }
}
