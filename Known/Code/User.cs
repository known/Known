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
using Known.Core;

namespace Known
{
    public class UserInfo
    {
        public string Host { get; set; }
        public string AppId { get; set; }
        public string AppName { get; set; }
        public string AppLang { get; set; }

        public string Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string EnglishName { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }
        public int Enabled { get; set; }
        public DateTime? FirstLoginTime { get; set; }
        public string FirstLoginIP { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public string LastLoginIP { get; set; }
        public string IPName { get; set; }
        public string Token { get; set; }
        public string AvatarUrl { get; set; }
        public string CompNo { get; set; }
        public string CompName { get; set; }
        public string OrgNo { get; set; }
        public string OrgName { get; set; }
        public bool IsGroupUser { get; set; }
        public string Role { get; set; }
        public string Extension { get; set; }

        internal bool IsAdmin
        {
            get { return UserName == Constants.SysUserName.ToLower(); }
        }

        public static void AddToken(UserInfo user)
        {
            UserHelper.AddToken(user);
        }

        public static UserInfo GetUser(out string error)
        {
            return UserHelper.GetUser(out error);
        }
    }

    public sealed class UserHelper
    {
        private const string KEY_CUR_USER = "Key_CurrentUser";
        private const string KEY_CUR_MENU = "Key_CurrentUserMenus";
        private const string KEY_TOKEN = "Key_Token_{0}";
        private const string KEY_CLIENT = "Key_Client_{0}";
        private static readonly IAppContext context = DefaultAppContext.Current;
        private static readonly bool checkClient = Config.App.Param("CheckClient", true);

        static UserHelper()
        {
            var app = Config.App;
            var user = new UserInfo
            {
                CompNo = app.CompNo,
                AppId = app.AppId,
                AppName = app.AppName,
                Token = Config.MacAddress,
                UserName = "local",
                Name = Language.LocalUser
            };
            AddToken(user);
        }

        private UserHelper() { }

        public static void AddToken(UserInfo user)
        {
            var key = string.Format(KEY_TOKEN, user.Token);
            Cache.Set(key, user);
        }

        public static UserInfo GetUser(out string error)
        {
            try
            {
                error = string.Empty;
                var user = context.GetSession<UserInfo>(KEY_CUR_USER);
                if (user == null)
                {
                    var token = context.GetRequest(Constants.KeyToken);
                    if (!string.IsNullOrEmpty(token))
                    {
                        user = GetUserByToken(token, out error);
                    }
                }

                if (user != null)
                {
                    if (checkClient)
                    {
                        var clientId = context.GetRequest(Constants.KeyClient);
                        if (!string.IsNullOrEmpty(clientId))
                        {
                            var isApp = context.CheckMobile();
                            var key = GetClientKey(user, isApp);
                            var client = Cache.Get<string>(key);
                            if (!string.IsNullOrEmpty(client) && client != clientId)
                            {
                                error = Language.LoginOffline;
                                return null;
                            }
                        }
                    }
                }

                return user;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }
        }

        private static UserInfo GetUserByToken(string token, out string error)
        {
            var key = string.Format(KEY_TOKEN, token);
            var user = Cache.Get<UserInfo>(key);
            if (user == null)
            {
                error = Language.NoLogin;
                Cache.Remove(key);
                return null;
            }

            error = string.Empty;
            return user;
        }

        internal static string GetUserNameByToken(string token)
        {
            var tokens = Config.App.Param<string>("Tokens");
            if (string.IsNullOrEmpty(tokens))
                return null;

            var userName = string.Empty;
            var values = tokens.Split('|');
            foreach (var item in values)
            {
                var items = item.Split('-');
                if (items[0] == token)
                {
                    userName = items[1];
                    break;
                }
            }

            return userName;
        }

        internal static UserInfo GetTokenUser(string userName)
        {
            var tokenUsers = Cache.GetList<UserInfo>("Key_Token_");
            return tokenUsers.FirstOrDefault(u => u.UserName == userName);
        }

        internal static void SetUser(UserInfo user)
        {
            context.SetSession(KEY_CUR_USER, user);

            if (!string.IsNullOrEmpty(user.Token))
            {
                var key = string.Format(KEY_TOKEN, user.Token);
                Cache.Set(key, user);
            }
        }

        internal static void RemoveUser(string userName)
        {
            var user = GetTokenUser(userName);
            if (user != null)
            {
                var isApp = context.CheckMobile();
                var key = GetClientKey(user, isApp);
                Cache.Remove(key);
            }
        }

        internal static Result CheckClient(string clientId, UserInfo user, bool isApp, bool force)
        {
            if (!checkClient || string.IsNullOrEmpty(clientId))
                return Result.Success("");

            var key = GetClientKey(user, isApp);
            var value = Cache.Get<string>(key);
            if (force)
                value = clientId;

            if (!string.IsNullOrEmpty(value) && value != clientId)
            {
                var message = Language.LoginOneAccount.Format(user.LastLoginIP);
                return Result.Error(message, new { Confirm = true });
            }

            Cache.Set(key, clientId);
            return Result.Success("");
        }

        private static string GetClientKey(UserInfo user, bool isApp)
        {
            var type = isApp ? "APP" : "WEB";
            var name = $"{type}_{user.UserName}";
            return string.Format(KEY_CLIENT, name);
        }

        internal static List<MenuInfo> GetMenus()
        {
            return context.GetSession<List<MenuInfo>>(KEY_CUR_MENU);
        }

        internal static void SetMenus(List<MenuInfo> menus)
        {
            context.SetSession(KEY_CUR_MENU, menus);
        }
    }
}