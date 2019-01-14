using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Known.Platform;

namespace Known.WebMvc
{
    public class WebMvcConfig
    {
        public static void Register()
        {
            Environment.CurrentDirectory = HttpRuntime.AppDomainAppPath;
            Container.Register<IJsonProvider, JsonProvider>();
            Container.Register<IPlatformRepository, PlatformRepository>();

            AreaRegistration.RegisterAllAreas();

            FilterConfig.Register(GlobalFilters.Filters);
            RouteConfig.Register(RouteTable.Routes);
        }
    }
}
