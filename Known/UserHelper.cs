using System.Collections.Generic;

namespace Known
{
    public sealed class UserHelper
    {
        private const string KEY_CUR_USER = "Key_CurrentUser";
        private static readonly Dictionary<string, UserInfo> tokenUsers = new Dictionary<string, UserInfo>();

        private UserHelper() { }

        public static UserInfo GetUser(out string error)
        {
            var token = AppContext.Current.GetRequestHeader("token");
            if (!string.IsNullOrWhiteSpace(token))
                return GetUserByToken(token, out error);

            error = string.Empty;
            return AppContext.Current.GetSession<UserInfo>(KEY_CUR_USER);
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

            error = string.Empty;
            return user;
        }

        internal static void SetUser(UserInfo user)
        {
            AppContext.Current.SetSession(KEY_CUR_USER, user);

            if (!string.IsNullOrWhiteSpace(user.Token))
                tokenUsers[user.Token] = user;
        }
    }
}
