using System.Web.UI;
using Known.Web.Extensions;

namespace Known.Web
{
    /// <summary>
    /// Web 窗体页面基类。
    /// </summary>
    public class BasePage : Page
    {
        private ApiClient api;

        /// <summary>
        /// 取得 Api 客户端对象。
        /// </summary>
        public ApiClient Api
        {
            get
            {
                if (api == null)
                {
                    api = new ApiClient(null, UserToken);
                }
                return api;
            }
        }

        /// <summary>
        /// 取得或设置用户身份认证票据。
        /// </summary>
        public string UserToken
        {
            get { return Session.GetValue<string>("UserToken"); }
            set { Session.SetValue("UserToken", value); }
        }

        /// <summary>
        /// 取得当前登录的用户名。
        /// </summary>
        protected string UserName
        {
            get { return User.Identity.Name; }
        }

        /// <summary>
        /// 取得当前用户是否已经验证。
        /// </summary>
        protected bool IsAuthenticated
        {
            get { return User.Identity.IsAuthenticated; }
        }
    }
}
