using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;

namespace Known.Web
{
    public class WebApplication : HttpApplication
    {
        protected virtual void Application_Start(object sender, EventArgs e)
        {
            Environment.CurrentDirectory = HttpRuntime.AppDomainAppPath;
            InitialModules();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            var message = $"操作人：{User.Identity.Name}，地址：{Request.Url}，错误：{Environment.NewLine}{ex}";
            Logger.Error(message);
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }

        private void InitialModules()
        {
            var assemblies = BuildManager.GetReferencedAssemblies()
                                         .Cast<Assembly>().ToList();
            foreach (var item in assemblies)
            {
                var ns = item.FullName.Split(',')[0];
                var type = item.GetType($"{ns}.Initializer");
                if (type == null)
                    continue;

                var method = type.GetMethod("Initialize");
                if (method != null)
                {
                    method.Invoke(null, null);
                }
            }
        }
    }
}
