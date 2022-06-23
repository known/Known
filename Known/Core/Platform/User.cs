/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * 2022-06-23     KnownChen    优化用户管理及登录
 * ------------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.Linq;

namespace Known.Core
{
    partial class PlatformService
    {
        public Result SignIn(string userName, string password, string cid = null, bool force = false)
        {
            var entity = Repository.GetUser(Database, userName, password);
            return SignIn(entity, cid, force);
        }

        public Result SignInByToken(string token)
        {
            var userName = UserHelper.GetUserNameByToken(token);
            if (string.IsNullOrEmpty(userName))
                return Result.Error("The token is invalid.");

            var entity = Repository.GetUser(Database, userName);
            return SignIn(entity, "");
        }

        private Result SignIn(UserInfo entity, string cid = null, bool force = false)
        {
            Context.ClearSession();

            if (entity == null)
                return Result.Error(Language.LoginNoNamePwd);

            if (entity.Enabled == 0)
                return Result.Error(Language.LoginDisabled);

            var isApp = Context.CheckMobile();
            var result = UserHelper.CheckClient(cid, entity, isApp, force);
            if (!result.IsValid)
                return result;

            var ip = Context.GetIPAddress();
            if (!entity.FirstLoginTime.HasValue)
            {
                entity.FirstLoginTime = DateTime.Now;
                entity.FirstLoginIP = ip;
            }
            entity.LastLoginTime = DateTime.Now;
            entity.LastLoginIP = ip;

            var type = Language.Login;
            if (isApp)
            {
                type = "APP" + type;
            }
            var user = GetUserInfo(entity);
            UserHelper.SetUser(user);

            return Database.Transaction(Language.Login, db =>
            {
                Repository.UpdateUser(db, entity);
                AddLoginLog(db, type, user);
            }, user);
        }

        internal UserInfo ReSignIn(string userName)
        {
            var entity = Repository.GetUser(Database, userName);
            if (entity == null)
                return null;

            var isApp = Context.CheckMobile();
            var ip = Context.GetIPAddress();
            if (!entity.FirstLoginTime.HasValue)
            {
                entity.FirstLoginTime = DateTime.Now;
                entity.FirstLoginIP = ip;
            }
            entity.LastLoginTime = DateTime.Now;
            entity.LastLoginIP = ip;

            var type = Language.ReLogin;
            if (isApp)
            {
                type = "APP" + type;
            }
            var user = GetUserInfo(entity);
            UserHelper.SetUser(user);

            Database.Transaction(Language.ReLogin, db =>
            {
                Repository.UpdateUser(db, entity);
                AddLoginLog(db, type, user);
            });

            return user;
        }

        private void AddLoginLog(Database db, string type, UserInfo user)
        {
            var browser = Context.GetBrowser();
            var note = $"姓名：{user.Name}；IP：{user.LastLoginIP}；所在地：{user.IPName}；浏览器：{browser.Platform} {browser.Browser}{browser.MajorVersion}";
            AddLog(db, user.AppId, user.CompNo, user.UserName, type, user.UserName, note);
        }

        internal bool CheckDevUser(UserInfo user)
        {
            return user.IsAdmin && (user.LastLoginIP == "127.0.0.1" || user.LastLoginIP == "::1");
        }

        public void SignOut(string userName)
        {
            Context.ClearSession();
            UserHelper.RemoveUser(userName);
        }

        public UserInfo GetUser(string userName)
        {
            return Repository.GetUser(Database, userName);
        }

        internal UserInfo GetUserInfo(string userName, bool isApp = false)
        {
            var user = UserHelper.GetUser(out _);
            if (user == null && !string.IsNullOrEmpty(userName))
            {
                var entity = Repository.GetUser(Database, userName);
                user = GetUserInfo(entity);
            }

            return user;
        }

        public List<MenuInfo> GetUserMenus(string appId, string userName, bool refresh = false)
        {
            var userMenus = UserHelper.GetMenus();
            if (userMenus == null || refresh)
            {
                if (userName == Constants.SysUserName.ToLower())
                    userMenus = Repository.GetMenus(Database, appId);
                else
                    userMenus = Repository.GetUserMenus(Database, appId, userName);

                UserHelper.SetMenus(userMenus);
            }

            return userMenus;
        }

        public Result SaveUserInfo(UserInfo model)
        {
            var user = CurrentUser;
            if (user == null)
                return Result.Error(Language.NoLogin);

            var entity = Repository.GetUserById(Database, model.Id);
            if (entity == null)
                return Result.Error(Language.NoUser);

            entity.Name = model.Name;
            entity.EnglishName = model.EnglishName;
            entity.Gender = model.Gender;
            entity.Phone = model.Phone;
            entity.Mobile = model.Mobile;
            entity.Email = model.Email;
            entity.Note = model.Note;
            Repository.UpdateUser(Database, entity);

            user.Name = model.Name;
            user.EnglishName = model.EnglishName;
            user.Gender = model.Gender;
            user.Phone = model.Phone;
            user.Mobile = model.Mobile;
            user.Email = model.Email;
            user.Note = model.Note;
            SetUserAvatar(user);

            return Result.Success(Language.XXSuccess.Format(Language.Save), entity.Id);
        }

        public Result UpdatePassword(string oldPwd, string newPwd, string newPwd1)
        {
            var user = CurrentUser;
            if (user == null)
                return Result.Error(Language.NoLogin);

            var errors = new List<string>();
            if (string.IsNullOrEmpty(oldPwd))
                errors.Add("当前密码不能为空！");
            if (string.IsNullOrEmpty(newPwd))
                errors.Add("新密码不能为空！");
            if (string.IsNullOrEmpty(newPwd1))
                errors.Add("确认新密码不能为空！");
            if (newPwd != newPwd1)
                errors.Add("两次密码输入不一致！");

            if (errors.Count > 0)
                return Result.Error(string.Join(Environment.NewLine, errors.ToArray()));

            var entity = Repository.GetUser(Database, user.UserName, oldPwd);
            if (entity == null)
                return Result.Error("当前密码不正确！");

            var password = Utils.ToMd5(newPwd);
            Repository.UpdatePassword(Database, entity.Id, password);
            return Result.Success(Language.XXSuccess.Format(Language.Update), entity.Id);
        }

        private UserInfo GetUserInfo(UserInfo user)
        {
            var tuser = UserHelper.GetTokenUser(user.UserName);
            user.Token = tuser != null ? tuser.Token : Utils.GetGuid();
            user.IsGroupUser = user.OrgNo == user.CompNo;
            user.Host = Context.Host;

            var app = Config.App;
            user.AppName = app.AppName;
            user.AppLang = app.AppLang;
            if (user.IsAdmin)
            {
                user.AppId = app.AppId;
            }

            var info = GetSystem();
            user.AppName = info.AppName;
            user.CompName = info.CompName;
            if (!string.IsNullOrEmpty(user.OrgNo))
            {
                var orgName = Repository.GetOrgName(Database, user.AppId, user.CompNo, user.OrgNo);
                if (string.IsNullOrEmpty(orgName))
                    orgName = user.CompName;
                user.OrgName = orgName;
                if (string.IsNullOrEmpty(user.CompName))
                    user.CompName = orgName;
            }

            SetUserAvatar(user);
            return user;
        }

        private static void SetUserAvatar(UserInfo user)
        {
            if (user == null)
                return;

            user.AvatarUrl = user.Gender == "女"
                           ? "/img/face2.png"
                           : "/img/face1.png";
        }
    }

    partial class PlatformRepository
    {
        public UserInfo GetUser(Database db, string userName)
        {
            var sql = "select * from SysUser where UserName=@userName";
            return db.Query<UserInfo>(sql, new { userName });
        }

        public UserInfo GetUser(Database db, string userName, string password)
        {
            password = Utils.ToMd5(password);
            var sql = "select * from SysUser where UserName=@userName and Password=@password";
            return db.Query<UserInfo>(sql, new { userName, password });
        }

        public UserInfo GetUserById(Database db, string id)
        {
            var sql = "select * from SysUser where Id=@id";
            return db.Query<UserInfo>(sql, new { id });
        }

        public List<MenuInfo> GetMenus(Database db, string appId)
        {
            if (Config.HasMenu)
                return Config.Menus.Where(m => m.AppId == appId).ToList();

            var sql = "select * from SysModule where Enabled=1 and AppId=@appId order by Sort";
            return db.QueryList<MenuInfo>(sql, new { appId });
        }

        public List<MenuInfo> GetUserMenus(Database db, string appId, string userName)
        {
            if (Config.HasMenu)
            {
                var sql1 = @"
select a.ModuleId from SysRoleModule a,SysRole b,SysUserRole c,SysUser d 
where a.RoleId=b.Id and b.Id=c.RoleId and c.UserId=d.Id and d.UserName=@userName 
union 
select a.ModuleId from SysUserModule a,SysUser b 
where a.UserId=b.Id and b.UserName=@userName";
                var ids = db.Scalars<string>(sql1, new { userName });
                return Config.Menus.Where(m => m.AppId == appId && ids.Contains(m.Id)).ToList();
            }

            var sql = @"
select * from SysModule 
where Enabled=1 and AppId=@appId and Id in (
  select a.ModuleId from SysRoleModule a,SysRole b,SysUserRole c,SysUser d
  where a.RoleId=b.Id and b.Id=c.RoleId and c.UserId=d.Id and d.UserName=@userName
  union 
  select a.ModuleId from SysUserModule a,SysUser b 
  where a.UserId=b.Id and b.UserName=@userName
)
order by Sort";
            return db.QueryList<MenuInfo>(sql, new { appId, userName });
        }

        public string GetOrgName(Database db, string appId, string compNo, string orgNo)
        {
            var sql = "select Name from SysOrganization where AppId=@appId and CompNo=@compNo and Code=@orgNo";
            return db.Scalar<string>(sql, new { appId, compNo, orgNo });
        }

        public void UpdateUser(Database db, UserInfo user)
        {
            var sql = @"
update SysUser 
set FirstLoginTime=@FirstLoginTime,FirstLoginIP=@FirstLoginIP
   ,LastLoginTime=@LastLoginTime,LastLoginIP=@LastLoginIP
   ,Name=@Name,EnglishName=@EnglishName,Gender=@Gender,Phone=@Phone
   ,Mobile=@Mobile,Email=@Email,Note=@Note 
where Id=@Id";
            db.Execute(sql, user);
        }

        public void UpdatePassword(Database db, string id, string password)
        {
            var sql = "update SysUser set Password=@password where Id=@id";
            db.Execute(sql, new { password, id });
        }
    }
}
