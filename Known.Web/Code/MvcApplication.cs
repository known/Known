using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Known.Web
{
    public class MvcApplication : WebApplication
    {
        protected override void Application_Start(object sender, EventArgs e)
        {
            base.Application_Start(sender, e);
            ViewEngines.Engines.RemoveAt(0);
            WebConfig.RegisterFilters(GlobalFilters.Filters);
            WebConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}