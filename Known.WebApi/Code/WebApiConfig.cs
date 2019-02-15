using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using Known.Platform;
using Known.Web;
using Known.WebApi.Filters;
using Newtonsoft.Json;

namespace Known.WebApi
{
    public class WebApiConfig
    {
        public static void Register()
        {
            Environment.CurrentDirectory = HttpRuntime.AppDomainAppPath;
            Container.Register<IJson, JsonProvider>();
            Container.Register<IPlatformRepository, PlatformRepository>();

            GlobalConfiguration.Configure(Register);
        }

        private static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.MessageHandlers.Add(new BasicAuthenticationHandler());
            config.MessageHandlers.Add(new DecompressionHandler());
            config.Filters.Add(new ApiLoginAuthorizeAttribute());
            config.Filters.Add(new ApiTrackActionAttribute());
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

        class PlatformRepository : IPlatformRepository
        {
            public string GetApiBaseUrl(string apiId)
            {
                var apiUrl = string.Empty;
                if (apiId == "plt")
                    apiUrl = "http://localhost:8089";

                return apiUrl;
            }

            public Dictionary<string, object> GetCodes(string appId)
            {
                return new Dictionary<string, object>();
            }

            public Module GetModule(string id)
            {
                return new Module { Id = id };
            }

            public List<Module> GetModules(string appId)
            {
                return new List<Module>();
            }

            public List<Module> GetUserModules(string appId, string userName)
            {
                return new List<Module>();
            }

            public User GetUser(string userName)
            {
                return new User
                {
                    UserName = userName,
                    Name = "测试",
                    Password = "c4ca4238a0b923820dcc509a6f75849b"
                };
            }

            public void SaveUser(User user)
            {
            }
        }
    }
}