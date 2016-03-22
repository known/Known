using System;
using Known.Web.Extensions;

namespace Known.Web.Admin
{
    public class Login : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session.Clear();

            if (Request.IsPost())
            {
                var userName = Request.Get<string>("userName");
                var password = Request.Get<string>("password");
                var message = string.Empty;
                var user = UserInfo.CheckUser(userName, password, out message);
                if (user == null)
                {
                    //AppContext.AlertMessage = message;
                }
                else
                {
                    //AppContext.CurrentUser = user;
                    //Response.Redirect(KConfig.AdminPath);
                }
            }
        }
    }
}
