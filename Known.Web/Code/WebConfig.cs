using System.Web.Mvc;
using System.Web.Routing;

namespace Known.Web
{
    class WebConfig
    {
        internal static void RegisterFilters(GlobalFilterCollection filters)
        {
            filters.Add(new LoginAuthorizeAttribute());
        }

        internal static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();
            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new string[] { "Known.Web.Controllers" }
            );
        }
    }
}
