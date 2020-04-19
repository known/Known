using System.Web;

namespace Known.Web
{
    public class SessionHelper
    {
        private const string KEY_CUR_USER = "Key_CurrentUser";

        public static UserInfo GetUser()
        {
            return HttpContext.Current.Session[KEY_CUR_USER] as UserInfo;
        }

        public static void SetUser(UserInfo user)
        {
            HttpContext.Current.Session[KEY_CUR_USER] = user;
        }
    }
}
