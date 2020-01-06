using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Mvc;
using System.Web.Routing;
using Known.Extensions;
using Known.Log;

namespace Known.Web
{
    /// <summary>
    /// Api应用程序全局类。
    /// </summary>
    public class MvcApplication : HttpApplication
    {
        /// <summary>
        /// 日志组件。
        /// </summary>
        protected readonly ILogger logger = new ConsoleLogger();

        /// <summary>
        /// 应用程序启动事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Application_Start(object sender, EventArgs e)
        {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            Container.Register<IJson, JsonProvider>();
            InitializeApp();
            Configuration();
        }

        /// <summary>
        /// 应用程序请求处理。
        /// </summary>
        protected virtual void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("Server");
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Remove("X-AspNetMvc-Version");
        }

        /// <summary>
        /// 应用程序认证请求事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 应用程序开始请求事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            Response.Write(new { ok = false, message }.ToJson());
            Response.End();
        }

        /// <summary>
        /// 应用程序发生错误事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            if (ex != null)
            {
                var message = $"App发生异常，操作人：{User.Identity.Name}，地址：{HttpContext.Current.Request.Url}";
                logger.Error(message, ex);
                ErrorResult(ex.Message);
            }
        }

        /// <summary>
        /// 应用程序停止事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Application_End(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 应用程序建立会话事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Session_Start(object sender, EventArgs e)
        {
            var sessionId = Session.SessionID;
        }

        /// <summary>
        /// 应用程序会话结束事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Session_End(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 应用程序配置。
        /// </summary>
        protected virtual void Configuration()
        {
            AreaRegistration.RegisterAllAreas();

            RouteTable.Routes.MapMvcAttributeRoutes();
            RouteTable.Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            RouteTable.Routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            GlobalFilters.Filters.Add(new MvcAuthorizeAttribute());
        }

        private void InitializeApp()
        {
            var context = Known.Context.Create();
            var assemblies = BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToList();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetExportedTypes();
                if (types == null || types.Length == 0)
                    continue;

                foreach (var type in types)
                {
                    InitializeModule(context, type);
                    InitializeMenu(type);
                }
            }
        }

        private static void InitializeModule(Context context, Type type)
        {
            if (type.IsSubclassOf(typeof(Initializer)))
            {
                var initializer = (Initializer)Activator.CreateInstance(type);
                if (initializer != null)
                {
                    initializer.Initialize(context);
                }
            }
        }

        private static void InitializeMenu(Type type)
        {
            var module = type.GetAttribute<ModuleAttribute>();
            if (module == null)
                return;

            var info = module.ToInfo(type.Name.Replace("Controller", ""));
            var methods = type.GetMethods();
            if (methods != null && methods.Length > 0)
            {
                foreach (var method in methods)
                {
                    var page = method.GetAttribute<PageAttribute>();
                    if (page != null)
                    {
                        var pi = page.ToInfo(method.Name);
                        pi.Url = $"/{info.Id}/{pi.Id}";
                        info.AddChild(pi);
                    }
                }
            }

            Setting.Instance.App.AddModule(info);
        }
    }
}
