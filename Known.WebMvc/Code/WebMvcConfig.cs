using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Known.WebMvc.Filters;

namespace Known.WebMvc
{
    public class WebMvcConfig
    {
        public static void Register()
        {
            Environment.CurrentDirectory = HttpRuntime.AppDomainAppPath;

            AreaRegistration.RegisterAllAreas();

            Register(GlobalFilters.Filters);
            Register(RouteTable.Routes);
        }

        private static void Register(GlobalFilterCollection filters)
        {
            filters.Add(new ValidateInputAttribute(false));
            filters.Add(new TrackActionAttribute());
            filters.Add(new AntiForgeryAttribute());
        }

        private static void Register(RouteCollection routes)
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
