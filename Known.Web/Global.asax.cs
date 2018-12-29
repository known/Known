using System;
using System.Web;
using Known.Platform;
using Known.Providers;
using Known.Serialization;
using Known.WebMvc;

namespace Known.Web
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            Environment.CurrentDirectory = HttpRuntime.AppDomainAppPath;
            Container.Register<IJsonProvider, JsonProvider>();
            Initializer.Initialize(Known.Context.Create());
            WebMvcConfig.Register();
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