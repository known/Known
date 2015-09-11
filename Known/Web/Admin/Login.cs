using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

using Known.Models;

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
                    AppContext.AlertMessage = message;
                }
                else
                {
                    AppContext.CurrentUser = user;
                    Response.Redirect(KConfig.AdminPath);
                }
            }
        }
    }
}
