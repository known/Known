using System;
using System.Text;
using System.Threading;
using System.Web;
using Known.Log;

namespace Known.Web.Mvc
{
    /// <summary>
    /// Http模块。
    /// </summary>
    public class HttpModule: IHttpModule
    {
        private HttpContext context;

        /// <summary>
        /// 初始化Http应用程序。
        /// </summary>
        /// <param name="context">应用程序。</param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += Context_BeginRequest;
            context.PostMapRequestHandler += Context_PostMapRequestHandler;
            context.AcquireRequestState += Context_AcquireRequestState;
            context.Error += Context_Error;
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        public void Dispose()
        {
        }

        private void Context_BeginRequest(object sender, EventArgs e)
        {
            var app = sender as HttpApplication;
            context = app.Context;
            if (context.Request.Url.LocalPath == "/")
            {
                context.RewritePath("/Home/Index");
            }
        }

        private void Context_PostMapRequestHandler(object sender, EventArgs e)
        {
            context.Handler = SessionHandler.Instance;
        }

        private void Context_AcquireRequestState(object sender, EventArgs e)
        {
            context.Response.HeaderEncoding = Encoding.Default;
            context.Response.ContentEncoding = Encoding.Default;

            InvokeAction();
        }

        private void Context_Error(object sender, EventArgs e)
        {
            LogHelper.Error(HttpContext.Current.Error.ToString());
        }

        private void InvokeAction()
        {
            var items = HttpContext.Current.Request.Url.LocalPath.Trim('/').Split('/');
            var controllerName = items[0];
            var type = ClassHelper.GetController(controllerName);
            if (type == null)
                WriteError($"{controllerName}控制器不存在！");

            try
            {
                var instance = Activator.CreateInstance(type) as Controller;
                instance.ProcessRequest(context);
            }
            catch (ThreadAbortException e)
            {
                //提前终止线程
            }
            catch (Exception ex)
            {
                WriteError(ex.Message);
            }
        }

        private void WriteError(string message)
        {
            context.Response.Write(message);
            context.Response.End();
        }
    }
}
