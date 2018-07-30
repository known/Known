using System.Web.UI;
using Known.Web.Extensions;

namespace Known.Web
{
    public class PageBase : Page
    {
        private ApiClient api;

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

        public string UserToken
        {
            get { return Session.GetValue<string>("UserToken"); }
            set { Session.SetValue("UserToken", value); }
        }

        protected string UserName
        {
            get { return User.Identity.Name; }
        }

        protected bool IsAuthenticated
        {
            get { return User.Identity.IsAuthenticated; }
        }
    }
}
