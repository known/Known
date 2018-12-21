using System.Web.Http;
using Known.Web;
using Known.WebApi.Filters;

namespace Known.WebApi
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.MessageHandlers.Add(new BasicAuthenticationHandler());
            config.MessageHandlers.Add(new DecompressionHandler());
            config.Filters.Add(new ApiLoginAuthorizeAttribute());
            config.Filters.Add(new ApiTrackActionAttribute());
        }
    }
}