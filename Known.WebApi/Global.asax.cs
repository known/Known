using System;
using Known.Data;
using Known.Platform;

namespace Known.WebApi
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            WebApiConfig.Register();

            var repository = new PlatformRepository(new Database());
            Container.Register<IPlatformRepository>(repository);
            Core.Initializer.Initialize();
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