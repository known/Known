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
            var Roles = Repository.QueryListById<Role>(ids);
            if (Roles == null || Roles.Count == 0)
                return Result.Error("请至少选择一条记录进行操作！");

            var info = Repository.Transaction("删除", rep =>
            {
                foreach (var item in Roles)
                {
                    rep.Delete(item);
                }
            });

            return info;
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
