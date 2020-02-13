using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using Known.Extensions;
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
        }

        private void Context_PostMapRequestHandler(object sender, EventArgs e)
        {
            context.Handler = SessionHandler.Instance;
        }

        private void Context_AcquireRequestState(object sender, EventArgs e)
        {
            context.Response.HeaderEncoding = Encoding.Default;
            context.Response.ContentEncoding = Encoding.Default;

            var url = context.Request.Url.LocalPath.Trim('/');
            InvokeAction(url);
        }

        private void Context_Error(object sender, EventArgs e)
        {
            var user = context.User.Identity.Name;
            var error = context.Error.ToString();
            LogHelper.Error($"{user} - {error}");
        }

        private void InvokeAction(string url)
        {
            if (url.Contains("static"))
                return;

            if (string.IsNullOrWhiteSpace(url))
                url = "Home/Index";

            var items = url.Split('/');
            var controllerName = items.Length > 0 ? items[0] : "Home";
            var type = ClassHelper.GetController(controllerName);
            if (type == null)
            {
                context.Response.Write($"{controllerName}Controller不存在！");
                return;
            }

            var actionName = items.Length > 1 ? items[1] : "Index";
            var method = type.GetMethod(actionName);
            if (method == null)
            {
                context.Response.Write($"{controllerName}Controller.{actionName}不存在！");
                return;
            }

            var id = items.Length > 2 ? items[2] : string.Empty;
            InvokeAction(type, method, id);
        }

        private void InvokeAction(Type type, MethodInfo method, string id)
        {
            try
            {
                var queries = HttpUtility.ParseQueryString(context.Request.Url.Query);
                var datas = queries.ToDictionary();
                if (!string.IsNullOrWhiteSpace(id))
                    datas["id"] = id;

                var obj = Activator.CreateInstance(type) as Controller;
                obj.Context = new ControllerContext(context, type, method);

                var result = InvokeAction(obj, method, datas);
                if (result != null)
                {
                    if (result is ActionResult ar)
                        ar.Execute();
                    else
                        context.Response.Write(result.ToString());
                }
            }
            catch (ThreadAbortException)
            {
                //Reponse.End提前终止线程
            }
            catch (Exception ex)
            {
                context.Response.Write(ex.Message);
            }
        }

        private object InvokeAction(Controller obj, MethodInfo method, Dictionary<string, object> datas)
        {
            var parameterInfos = method.GetParameters();
            if (parameterInfos == null || parameterInfos.Length == 0)
                return method.Invoke(obj, null);

            var parameters = new List<object>();
            if (datas == null || datas.Count == 0)
                throw new Exception($"{method.Name}参数不能为空！");

            foreach (var item in parameterInfos)
            {
                if (datas.ContainsKey(item.Name))
                    parameters.Add(datas[item.Name]);
                else
                    parameters.Add(null);
            }

            return method.Invoke(obj, parameters.ToArray());
        }
    }
}
