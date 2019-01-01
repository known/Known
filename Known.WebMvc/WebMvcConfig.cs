using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Known.Serialization;
using Known.WebMvc.Providers;

namespace Known.WebMvc
{
    public class WebMvcConfig
    {
        public static void Register()
        {
            Environment.CurrentDirectory = HttpRuntime.AppDomainAppPath;
            Container.Register<IJsonProvider, JsonProvider>();

            AreaRegistration.RegisterAllAreas();

            FilterConfig.Register(GlobalFilters.Filters);
            RouteConfig.Register(RouteTable.Routes);
        }
    }
}
