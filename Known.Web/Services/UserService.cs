using System.Collections.Generic;
using Known.Web.Datas;
using Known.Web.Entities;
using Known.Mapping;

namespace Known.Web.Services
{
    public class UserService : CoreServiceBase
    {
        public UserService(Context context) : base(context)
        {
        }

        #region View
        public PagingResult QueryUsers(PagingCriteria criteria)
        {
            return Database.QueryUsers(criteria);
        }

        public Result DeleteUsers(string[] ids)
        {
            var message = CheckEntities(ids, out List<TUser> users);
            if (!string.IsNullOrWhiteSpace(message))
                return Result.Error(message);

            return Database.Transaction("删除", db =>
            {
                foreach (var item in users)
                {
                    //rep.DeleteUserRoles(item.Id);
                    //rep.DeleteUserFunctions(item.Id);
                    db.Delete(item);
                }
            });
        }
        #endregion

        #region Form
        public TUser GetUser(string id)
        {
            return Database.QueryById<TUser>(id);
        }

        public Result SaveUser(dynamic model)
        {
            var id = (string)model.Id;
            var entity = Database.QueryById<TUser>(id);
            if (entity == null)
                entity = new TUser();

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
