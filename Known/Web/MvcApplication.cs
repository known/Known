using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Known.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            Environment.CurrentDirectory = HttpRuntime.AppDomainAppPath;
            ViewEngines.Engines.RemoveAt(0);
            WebConfig.RegisterFilters(GlobalFilters.Filters);
            WebConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            var message = $"操作人：{User.Identity.Name}，地址：{Request.Url}，错误：{Environment.NewLine}{ex}";
            Logger.Error(message);
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }
    }
}
