using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Http;
using Known.Extensions;
using Known.Log;

namespace Known.Web
{
    /// <summary>
    /// Api应用程序全局类。
    /// </summary>
    public class ApiApplication : HttpApplication
    {
        protected readonly ILogger logger = new TraceLogger();

        protected virtual void Application_Start(object sender, EventArgs e)
        {
            InitialModules();
        }

        protected virtual void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("Server");
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Remove("X-AspNetMvc-Version");
        }

        protected virtual void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected virtual void Application_BeginRequest(object sender, EventArgs e)
        {
            var checker = new XSSChecker(Request);
            if (Request.Cookies != null)
            {
                if (checker.CheckCookieData())
                    ErrorResult("您提交的数据有恶意字符！");
            }
            if (Request.UrlReferrer != null)
            {
                if (checker.CheckUrlReferer())
                    ErrorResult("您提交的数据有恶意字符！");
            }
            if (Request.RequestType.ToUpper() == "POST")
            {
                if (checker.CheckPostData())
                    ErrorResult("您提交的数据有恶意字符！");
            }
            if (Request.RequestType.ToUpper() == "GET")
            {
                if (checker.CheckGetData())
                    ErrorResult("您提交的数据有恶意字符！");
            }
        }

        private void ErrorResult(string message)
        {
            Response.Write(new { success = false, message }.ToJson());
            Response.End();
        }

        protected virtual void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            if (ex != null)
            {
                var message = $"App发生异常，操作人：{User.Identity.Name}，地址：{HttpContext.Current.Request.Url}";
                logger.Error(message, ex);

                Response.StatusCode = 200;
                ErrorResult(ex.Message);
            }
        }

        protected virtual void Application_End(object sender, EventArgs e)
        {
        }

        protected virtual void Session_Start(object sender, EventArgs e)
        {
        }

        protected virtual void Session_End(object sender, EventArgs e)
        {
        }

        protected virtual void Configuration(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.Remove(config.Formatters.XmlFormatter);

            config.MessageHandlers.Add(new DecompressionHandler());

            config.Filters.Add(new ApiLoginAuthorizeAttribute());
            config.Filters.Add(new ApiTrackActionAttribute());
        }

        private void InitialModules()
        {
            var assemblies = BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToList();
            foreach (var item in assemblies)
            {
                if (!item.FullName.StartsWith("Known"))
                    continue;

                var ns = item.FullName.Split(',')[0];
                var type = item.GetType($"{ns}.Initializer");
                if (type == null)
                    continue;

                var method = type.GetMethod("Initialize", BindingFlags.Public | BindingFlags.Static);
                if (method != null)
                {
                    method.Invoke(null, null);
                }
            }
        }
    }
}
