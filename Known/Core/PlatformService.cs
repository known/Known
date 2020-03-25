using System.Collections.Generic;

namespace Known.Core
{
    public class PlatformService : BaseService
    {
        private IPlatformRepository Repository
        {
            get { return Container.Resolve<IPlatformRepository>(); }
        }

        internal Result SignIn(string userName, string password)
        {
            var user = Repository.GetUser(Connection, userName);
            if (user == null)
                return Result.Error("用户名不存在！");

            if (user.Password != password)
                return Result.Error("密码不正确！");

            return Result.Success("登录成功！", user);
        }

        internal List<MenuInfo> GetUserMenus(string userName)
        {
            return Repository.GetUserMenus(Connection, userName);
        }
    }
}
