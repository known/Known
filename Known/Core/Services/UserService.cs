using Known.Core.Repositories;

namespace Known.Core.Services
{
    class UserService : CoreServiceBase<IUserRepository>
    {
        public UserService(Context context) : base(context)
        {
        }

        #region View
        public PagingResult QueryUsers(PagingCriteria criteria)
        {
            return Repository.QueryUsers(criteria);
        }

        public Result DeleteUsers(string[] ids)
        {
            var users = Repository.QueryListById<Entities.Module>(ids);
            if (users == null || users.Count == 0)
                return Result.Error("请至少选择一条记录进行操作！");

            var info = Repository.Transaction("删除", rep =>
            {
                foreach (var item in users)
                {
                    rep.Delete(item);
                }
            });

            return info;
        }
        #endregion

        #region Form
        #endregion
    }
}
