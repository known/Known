using System.Web.Mvc;
using System.Web.Routing;

namespace Known.Web
{
    public class RouteConfig
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