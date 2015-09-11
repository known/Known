using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Known.Web
{
    public class AdminPage : PageBase
    {
        public override string VirtualPath
        {
            get { return KConfig.AdminPath; }
        }

        protected override void ValidateLogin()
        {
            if (AppContext.CurrentUser == null)
            {
                var url = Utils.GetAdminUrl("Login.aspx");
                Response.Navigate(url);
            }
        }
    }
}
