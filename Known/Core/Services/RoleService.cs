using System.Collections.Generic;
using Known.Core.Entities;
using Known.Mapping;

namespace Known.Core
{
    class RoleService : CoreServiceBase<IRoleRepository>
    {
        public RoleService(Context context) : base(context)
        {
        }

        #region View
        public PagingResult QueryRoles(PagingCriteria criteria)
        {
            return Repository.QueryRoles(criteria);
        }

        public Result DeleteRoles(string[] ids)
        {
            var message = CheckEntities(ids, out List<Role> roles);
            if (!string.IsNullOrWhiteSpace(message))
                return Result.Error(message);

            return Repository.Transaction("删除", rep =>
            {
                foreach (var item in roles)
                {
                    //rep.DeleteRoleUsers(item.Id);
                    //rep.DeleteRoleFunctions(item.Id);
                    rep.Delete(item);
                }
            });
        }
        #endregion

        #region Form
        public Role GetRole(string id)
        {
            return Repository.QueryById<Role>(id);
        }

        public Result SaveRole(dynamic model)
        {
            if (model == null)
                return Result.Error("不能提交空数据！");

            var id = (string)model.Id;
            var entity = Repository.QueryById<Role>(id);
            if (entity == null)
                entity = new Role();

            EntityHelper.FillModel(entity, model);

            if (string.IsNullOrWhiteSpace(entity.AppId))
                entity.AppId = Setting.Instance.AppId;

            var vr = EntityHelper.Validate(entity);
            if (vr.HasError)
                return Result.Error(vr.ErrorMessage);

            Repository.Save(entity);
            return Result.Success("保存成功！", entity.Id);
        }
        #endregion
    }
}
