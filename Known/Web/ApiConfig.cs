using System.Web.Http;

namespace Known.Web
{
    /// <summary>
    /// Api应用程序配置。
    /// </summary>
    public sealed class ApiConfig
    {
        /// <summary>
        /// 注册应用程序配置。
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.MessageHandlers.Add(new DecompressionHandler());
            config.Filters.Add(new ApiLoginAuthorizeAttribute());
        }
    }
}
