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
using System.Text;
using System.Web;

namespace Known.Web
{
    public class HttpModule : IHttpModule
    {
        private HttpContext context;
        private readonly ProxyServer proxy = new ProxyServer();

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            Container.Register<IAppContext, WebAppContext>();
            ServiceHelper.Init();

            //var wrapper = new EventHandlerTaskAsyncHelper(AcquireRequestStateAsync);
            //context.AddOnAcquireRequestStateAsync(wrapper.BeginEventHandler, wrapper.EndEventHandler);

            context.BeginRequest += Context_BeginRequest;
            context.PostMapRequestHandler += Context_PostMapRequestHandler;
            context.PreSendRequestHeaders += Context_PreSendRequestHeaders;
            context.AcquireRequestState += Context_AcquireRequestState;
            context.Error += Context_Error;
        }

        private void Context_BeginRequest(object sender, EventArgs e)
        {
            context = ((HttpApplication)sender).Context;
            //WebAppContext.Context = context;
            proxy.Execute(context);
            //Config.InitSite(context.Request.Url);
        }

        private void Context_PostMapRequestHandler(object sender, EventArgs e)
        {
            context.Handler = SessionHandler.Instance;
            //context.SetSessionStateBehavior(SessionStateBehavior.ReadOnly);
        }

        private void Context_PreSendRequestHeaders(object sender, EventArgs e)
        {
            context.Response.Headers.Remove("Server");
            context.Response.Headers.Remove("X-AspNet-Version");
        }

        private void Context_AcquireRequestState(object sender, EventArgs e)
        {
            context.Response.HeaderEncoding = Encoding.UTF8;
            context.Response.ContentEncoding = Encoding.UTF8;
            ServiceHandler.Execute(context);
        }

        //private Task AcquireRequestStateAsync(object sender, EventArgs e)
        //{
        //    context.Response.HeaderEncoding = Encoding.UTF8;
        //    context.Response.ContentEncoding = Encoding.UTF8;
        //    //WebAppContext.Context = context;
        //    var url = context.Request.Url;
        //    Logger.Info($"{url} Start");
        //    await Task.Run(() =>
        //    {
        //        //ServiceHandler.Execute(context);
        //        Logger.Info($"{url} End");
        //    });
        //}

        private void Context_Error(object sender, EventArgs e)
        {
            var user = UserHelper.GetUser(out _);
            var userName = user != null ? user.UserName : "";
            var error = context.Error.ToString();
            Logger.Error($"{userName} - {error}");
        }
    }
}
#endif