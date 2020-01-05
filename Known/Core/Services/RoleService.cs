using System.Collections.Generic;
using Known.Core.Datas;
using Known.Core.Entities;
using Known.Mapping;

namespace Known.Core.Services
{
    class RoleService : CoreServiceBase
    {
        public RoleService(Context context) : base(context)
        {
        }

        #region View
        public PagingResult QueryRoles(PagingCriteria criteria)
        {
            return Database.QueryRoles(criteria);
        }

        public Result DeleteRoles(string[] ids)
        {
            var message = CheckEntities(ids, out List<TRole> roles);
            if (!string.IsNullOrWhiteSpace(message))
                return Result.Error(message);

            return Database.Transaction("删除", db =>
            {
                foreach (var item in roles)
                {
                    db.DeleteRoleUsers(item.Id);
                    db.DeleteRoleFunctions(item.Id);
                    db.Delete(item);
                }
            });
        }
        #endregion

        #region Form
        public TRole GetRole(string id)
        {
            return Database.QueryById<TRole>(id);
        }

        public Result SaveRole(dynamic model)
        {
            var id = (string)model.Id;
            var entity = Database.QueryById<TRole>(id);
            if (entity == null)
                entity = new TRole();

            EntityHelper.FillModel(entity, model);

            if (string.IsNullOrWhiteSpace(entity.AppId))
                entity.AppId = Setting.Instance.App.Id;

            var vr = EntityHelper.Validate(entity);
            if (vr.HasError)
                return Result.Error(vr.ErrorMessage);

            Database.Save(entity);
            return Result.Success("保存成功！", entity.Id);
        }
        #endregion
    }
}
