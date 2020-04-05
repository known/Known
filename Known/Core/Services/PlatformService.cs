using System;
using System.Collections.Generic;
using System.Web;
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

            var pwd = Utils.ToMd5(password);
            if (user.Password != pwd)
                return Result.Error("密码不正确！");

            var ip = Utils.GetIPAddress(HttpContext.Current.Request);
            if (!user.FirstLoginTime.HasValue)
            {
                user.FirstLoginTime = DateTime.Now;
                user.FirstLoginIP = ip;
            }
            user.LastLoginTime = DateTime.Now;
            user.LastLoginIP = ip;

            Database.Save(user);
            return Result.Success("登录成功！", Utils.MapTo<UserInfo>(user));
        }

        internal List<MenuInfo> GetUserMenus(string userName, string parentId)
        {
            return Repository.GetUserMenus(Database, userName, parentId);
        }
    }
}
