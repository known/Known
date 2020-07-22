using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace Known.Web
{
    public class MvcApplication : WebApplication
    {
        protected override void Application_Start(object sender, EventArgs e)
        {
            var assembly = typeof(MvcApplication).Assembly;
            var json = Utils.GetResource(assembly, "DevMenu");
            var menus = Utils.FromJson<List<MenuInfo>>(json);
            Cache.Set("DevMenu", menus);

            base.Application_Start(sender, e);
            ViewEngines.Engines.RemoveAt(0);
            AreaRegistration.RegisterAllAreas();
            WebConfig.RegisterFilters(GlobalFilters.Filters);
            WebConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}