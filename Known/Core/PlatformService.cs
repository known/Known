using System;
using System.Collections.Generic;
using System.Web;
using Known.Core.Entities;
using Known.Web;

namespace Known.Core
{
    public class PlatformService : ServiceBase
    {
        private IPlatformRepository Repository
        {
            get { return Container.Resolve<IPlatformRepository>(); }
        }

        public Result SignIn(string userName, string password)
        {
            var entity = Repository.GetUser(Database, userName);
            if (entity == null)
                return Result.Error("用户名不存在！");

            if (entity.Enabled == 0)
                return Result.Error("用户已禁用！");

            var pwd = Utils.ToMd5(password);
            if (entity.Password != pwd)
                return Result.Error("密码不正确！");

            var ip = Utils.GetIPAddress(HttpContext.Current.Request);
            if (!entity.FirstLoginTime.HasValue)
            {
                entity.FirstLoginTime = DateTime.Now;
                entity.FirstLoginIP = ip;
            }
            entity.LastLoginTime = DateTime.Now;
            entity.LastLoginIP = ip;

            var user = GetUserInfo(entity);
            user.Token = Utils.GetGuid();
            SessionHelper.SetUser(user);
            Database.Save(entity);
            return Result.Success("登录成功！", user);
        }

        public void SignOut(string userName)
        {
            //SessionHelper.Remove
        }

        public UserInfo GetUserInfo(string userName)
        {
            var user = SessionHelper.GetUser(out _);
            if (user == null && !string.IsNullOrWhiteSpace(userName))
            {
                var entity = Repository.GetUser(Database, userName);
                user = GetUserInfo(entity);
                SessionHelper.SetUser(user);
            }
            return user;
        }

        public List<MenuInfo> GetUserMenus(string userName)
        {
            if (userName == "System")
            {
                var menus = Repository.GetMenus(Database);
                var dev = Utils.GetResource(GetType().Assembly, "DevMenu");
                menus.InsertRange(0, Utils.FromJson<List<MenuInfo>>(dev));
                return menus;
            }

            return Repository.GetUserMenus(Database, userName);
        }

        private UserInfo GetUserInfo(SysUser entity)
        {
            var user = Utils.MapTo<UserInfo>(entity);
            user.CompName = Repository.GetOrgName(Database, Config.AppId, user.CompNo);
            user.OrgName = Repository.GetOrgName(Database, Config.AppId, user.OrgNo);
            return user;
        }
    }
}
