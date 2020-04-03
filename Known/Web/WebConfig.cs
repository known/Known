using System.Web.Mvc;
using System.Web.Routing;

namespace Known.Web
{
    class WebConfig
    {
        public static void RegisterFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new LoginAuthorizeAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();
            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
