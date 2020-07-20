using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;

namespace Known.Web
{
    public sealed class UserHelper
    {
        private const string KEY_CUR_USER = "Key_CurrentUser";
        private static readonly Dictionary<string, UserInfo> tokenUsers = new Dictionary<string, UserInfo>();

        private UserHelper() { }

        public static UserInfo GetUser(out string error)
        {
            var token = HttpContext.Current.Request.Headers["token"];
            if (!string.IsNullOrWhiteSpace(token))
                return GetUserByToken(token, out error);

            error = string.Empty;
            return HttpContext.Current.Session[KEY_CUR_USER] as UserInfo;
        }

        private static UserInfo GetUserByToken(string token, out string error)
        {
            if (!tokenUsers.ContainsKey(token))
            {
                error = "Token不存在！";
                return null;
            }

            var user = tokenUsers[token];
            if (user == null)
            {
                error = "用户未登录！";
                tokenUsers.Remove(token);
                return null;
            }

            var timeout = FormsAuthentication.Timeout;
            if (user.LastLoginTime.HasValue && user.LastLoginTime.Value.Add(timeout) < DateTime.Now)
            {
                error = "用户登录已超时！";
                tokenUsers.Remove(token);
                return null;
            }

            error = string.Empty;
            return user;
        }

        internal static void SetUser(UserInfo user)
        {
            if (HttpContext.Current.Session != null)
                HttpContext.Current.Session[KEY_CUR_USER] = user;
            if (!string.IsNullOrWhiteSpace(user.Token))
                tokenUsers[user.Token] = user;
        }
    }
}
