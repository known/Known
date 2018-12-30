using System.Web.Mvc;
using System.Web.Routing;

namespace Known.WebMvc
{
    public class RouteConfig
    {
        public static void Register(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );//.RouteHandler = new CustomRouteHandler();
        }
    }
}