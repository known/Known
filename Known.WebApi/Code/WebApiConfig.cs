using System;
using System.Web;
using System.Web.Http;
using Known.Web;
using Known.WebApi.Filters;

namespace Known.WebApi
{
    public class WebApiConfig
    {
        public static void Register()
        {
            Environment.CurrentDirectory = HttpRuntime.AppDomainAppPath;
            GlobalConfiguration.Configure(Register);
        }

        private static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.MessageHandlers.Add(new AuthenticationHandler("Token"));
            config.MessageHandlers.Add(new DecompressionHandler());
            config.Filters.Add(new ApiLoginAuthorizeAttribute());
            config.Filters.Add(new ApiTrackActionAttribute());
        }
    }
}