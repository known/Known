using System.Web;

namespace Known.Web
{
    class SessionHelper
    {
        private const string KEY_CUR_USER = "Key_CurrentUser";

        internal static UserInfo GetUser()
        {
            return HttpContext.Current.Session[KEY_CUR_USER] as UserInfo;
        }

        internal static void SetUser(UserInfo user)
        {
            HttpContext.Current.Session[KEY_CUR_USER] = user;
        }
    }
}
