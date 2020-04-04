using System.Collections.Generic;
using Known.Core.Datas;

namespace Known.Core.Services
{
    public class PlatformService : ServiceBase
    {
        private IPlatformRepository Repository
        {
            get { return Container.Resolve<IPlatformRepository>(); }
        }

        internal Result SignIn(string userName, string password)
        {
            var user = Repository.GetUser(Database, userName);
            if (user == null)
                return Result.Error("用户名不存在！");

            if (user.Password != password)
                return Result.Error("密码不正确！");

            return Result.Success("登录成功！", user);
        }

        internal List<MenuInfo> GetUserMenus(string userName, string parentId)
        {
            return Repository.GetUserMenus(Database, userName, parentId);
        }
    }
}
