using System;

namespace Known.WebApi
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            WebApiConfig.Register();
        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }
    }
}