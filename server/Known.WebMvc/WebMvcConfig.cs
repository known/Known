using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace Known.WebMvc
{
    public class WebMvcConfig
    {
        public static void Register()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.Register(GlobalFilters.Filters);
            RouteConfig.Register(RouteTable.Routes);
        }
    }
}
