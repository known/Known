using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Known.Web
{
    /// <summary>
    /// 应用程序全局事件。
    /// </summary>
    public class Global : System.Web.HttpApplication
    {
        /// <summary>
        /// 应用程序启动时执行事件。
        /// </summary>
        /// <param name="sender">事件触发者。</param>
        /// <param name="e">事件参数。</param>
        protected void Application_Start(object sender, EventArgs e)
        {
            Environment.CurrentDirectory = HttpRuntime.AppDomainAppPath;
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        /// <summary>
        /// 应用程序错误捕获事件。
        /// </summary>
        /// <param name="sender">事件触发者。</param>
        /// <param name="e">事件参数。</param>
        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            if (ex != null)
            {
                Mail.SendException("Web程序发生异常", ex);
            }
        }

        class RouteConfig
        {
            public static void RegisterRoutes(RouteCollection routes)
            {
                routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

                routes.MapRoute("Login", "login", new { controller = "User", action = "Login" });
                routes.MapRoute("Register", "register", new { controller = "User", action = "Register" });
                routes.MapRoute("ForgotPassword", "forgotpwd", new { controller = "User", action = "ForgotPassword" });

                routes.MapRoute(
                    "Default",
                    "{controller}/{action}/{id}",
                    new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                );
            }
        }
    }
}