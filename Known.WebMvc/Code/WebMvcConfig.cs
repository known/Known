using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Known.WebMvc.Filters;
using Newtonsoft.Json;

namespace Known.WebMvc
{
    public class WebMvcConfig
    {
        public static void Register()
        {
            Environment.CurrentDirectory = HttpRuntime.AppDomainAppPath;
            Container.Register<IJson, JsonProvider>();

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

        class JsonProvider : IJson
        {
            public string Serialize<T>(T value)
            {
                return JsonConvert.SerializeObject(value);
            }

            public T Deserialize<T>(string json)
            {
                if (string.IsNullOrWhiteSpace(json))
                    return default(T);

                var settings = new JsonSerializerSettings
                {
                    DateFormatString = "yyyy-MM-dd HH:mm:ss"
                };
                return JsonConvert.DeserializeObject<T>(json, settings);
            }
        }
    }
}
