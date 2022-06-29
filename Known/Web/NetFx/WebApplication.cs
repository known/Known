/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * ------------------------------------------------------------------------------- */

#if !NET6_0
using System;
using System.Web;
using System.Web.Routing;

namespace Known.Web
{
    public class WebApplication : HttpApplication
    {
        private readonly ProxyServer proxy = new ProxyServer();

        protected virtual void Application_Start(object sender, EventArgs e)
        {
            Container.Register<AppContext, WebAppContext>();
            ServiceHelper.Init();
            RegisterRoutes(RouteTable.Routes);
        }

        protected virtual void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("Server");
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Remove("X-AspNetMvc-Version");
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            var sessionId = Session.SessionID;
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            proxy.Execute(Context);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            Logger.Exception(new LogInfo
            {
                User = User.Identity.Name,
                Url = Request.Url.ToString()
            }, ex);
        }

        private static void RegisterRoutes(RouteCollection routes)
        {
            //routes.Ignore("static/{*pathInfo}");

            routes.Add(new MvcRoute("{page}", new RouteValueDictionary(new { page = "" })));
            routes.Add(new MvcRoute("{app}/{service}/{action}"));
            routes.Add(new MvcRoute("{service}/{action}"));
        }
    }
}
#endif