using System;
using System.Web;
using System.Web.Http;

namespace Known.WebApi
{
    /// <summary>
    /// 应用程序全局事件。
    /// </summary>
    public class Global : HttpApplication
    {
        /// <summary>
        /// 应用程序启动时执行事件。
        /// </summary>
        /// <param name="sender">事件触发者。</param>
        /// <param name="e">事件参数。</param>
        protected void Application_Start(object sender, EventArgs e)
        {
            Environment.CurrentDirectory = HttpRuntime.AppDomainAppPath;
            GlobalConfiguration.Configure(WebApiConfig.Register);
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
                Mail.Send("WebApi程序发生异常", ex);
            }
        }

        class WebApiConfig
        {
            public static void Register(HttpConfiguration config)
            {
                config.MapHttpAttributeRoutes();

                config.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "api/{controller}/{action}/{id}",
                    defaults: new { id = RouteParameter.Optional }
                );

                config.Routes.MapHttpRoute(
                    name: "Error404",
                    routeTemplate: "{*url}",
                    defaults: new { controller = "Error", action = "Handle404" }
                );

                config.Formatters.Remove(config.Formatters.XmlFormatter);
            }
        }
    }
}