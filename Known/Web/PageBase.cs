using System.Web.UI;
using Known.Web.Extensions;

namespace Known.Web
{
    /// <summary>
    /// ASP.NET WebForm页面基类。
    /// </summary>
    public class PageBase : Page
    {
        private ApiClient api;

        /// <summary>
        /// 取得当前登录用户认证的Api客户端。
        /// </summary>
        public ApiClient Api
        {
            get
            {
                if (api == null)
                    api = new ApiClient(UserToken);

                return api;
            }
        }

        /// <summary>
        /// 取得或设置用户身份认证Token。
        /// </summary>
        public string UserToken
        {
            get { return Session.GetValue<string>("UserToken"); }
            set { Session.SetValue("UserToken", value); }
        }

        /// <summary>
        /// 取得当前登录的用户账号。
        /// </summary>
        protected string UserName
        {
            get { return User.Identity.Name; }
        }

        /// <summary>
        /// 取得当前用户是否已认证。
        /// </summary>
        protected bool IsAuthenticated
        {
            get { return User.Identity.IsAuthenticated; }
        }
    }
}
