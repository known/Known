using System;
using System.Web;
using System.Web.Http;
using Known.Providers;

namespace Known.WebApi
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            Environment.CurrentDirectory = HttpRuntime.AppDomainAppPath;
            ProviderConfig.RegisterProviders();

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            if (ex != null)
            {
                Mail.Send("Web程序发生异常", ex);
            }
        }
    }
}