using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Web;
using Known.Extensions;
using Known.Log;
using Known.Web.Extensions;

namespace Known.Web.Mvc
{
    /// <summary>
    /// Http模块。
    /// </summary>
    public class HttpModule : IHttpModule
    {
        private HttpContext context;

        /// <summary>
        /// 初始化Http应用程序。
        /// </summary>
        /// <param name="context">应用程序。</param>
        public void Init(HttpApplication context)
        {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            WebApp.Init(AppInfo.Instance);
            context.BeginRequest += Context_BeginRequest;
            context.PostMapRequestHandler += Context_PostMapRequestHandler;
            context.AcquireRequestState += Context_AcquireRequestState;
            context.EndRequest += Context_EndRequest;
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

            var checker = new XSSChecker(context.Request);
            if (!checker.Check(out string error))
                ErrorResult(error);
        }

        private void Context_PostMapRequestHandler(object sender, EventArgs e)
        {
            context.Handler = SessionHandler.Instance;
        }

        private void Context_AcquireRequestState(object sender, EventArgs e)
        {
            var url = context.Request.Url.LocalPath.Trim('/');
            InvokeAction(url);
        }

        private void Context_EndRequest(object sender, EventArgs e)
        {
            context.Response.Headers.Remove("Server");
            context.Response.Headers.Remove("X-AspNet-Version");
            context.Response.Headers.Remove("X-AspNetMvc-Version");

            context.Response.HeaderEncoding = Encoding.Default;
            context.Response.ContentEncoding = Encoding.Default;

            //if (!isAuthenticated)
            //    ErrorResult("未授权！", 401);
        }

        private void Context_Error(object sender, EventArgs e)
        {
            var user = context.User != null ? context.User.Identity.Name : "Anonymous";
            var url = context.Request.Url;
            var error = context.Error;
            var message = context.Error.ToString();
            LogHelper.Error($"{user} - {url} - {message}");
            if (!Setting.IsDebug)
            {
                ErrorResult(error.Message);
            }
        }

        private void ErrorResult(string message, int status = 200)
        {
            context.Response.StatusCode = status;

            if (context.Request.IsAjaxRequest())
            {
                context.Response.ContentType = MimeTypes.ApplicationJson;
                context.Response.Write(new { ok = false, message }.ToJson());
            }
            else
            {
                if (status == 401)
                    context.Response.Redirect("/login");
                else
                    context.Response.Write(message);
            }

            context.Response.End();
        }

        private void InvokeAction(string url)
        {
            if (url.Contains("static"))
                return;

            var action = WebApp.GetAction(url);
            if (action == null || action.Controller == null || action.Method == null)
            {
                context.Response.Write($"{url}不存在！");
                return;
            }

            InvokeAction(action);
        }

        private void InvokeAction(ActionInfo action)
        {
            try
            {
                var queries = HttpUtility.ParseQueryString(context.Request.Url.Query);
                action.QueryDatas = queries.ToDictionary();
                action.FormDatas = GetPostData(context.Request);
                action.RequestType = context.Request.RequestType;

                var obj = Activator.CreateInstance(action.Controller) as Controller;
                obj.Context = new ControllerContext(context, action);

                var result = InvokeAction(obj, action);
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
        }

        private object InvokeAction(Controller obj, ActionInfo action)
        {
            var method = action.Method;
            var parameters = method.GetParameters();
            if (parameters == null || parameters.Length == 0)
                return method.Invoke(obj, null);

            var querys = action.QueryDatas ?? new Dictionary<string, object>();
            var forms = action.FormDatas ?? new Dictionary<string, string>();
            if (querys.Count == 0 && forms.Count == 0)
                throw new Exception($"{method.Name}参数不能为空！");

            var args = new List<object>();
            foreach (var item in parameters)
            {
                if (querys.ContainsKey(item.Name))
                {
                    args.Add(querys[item.Name]);
                }
                else if (forms.ContainsKey(item.Name))
                {
                    args.Add(forms[item.Name]);
                }
                else
                {
                    if (item.ParameterType.IsClass)
                    {
                        var data = forms.ToJson().FromJson(item.ParameterType);
                        args.Add(data);
                    }
                    else
                    {
                        args.Add(null);
                    }
                }
            }

            return method.Invoke(obj, args.ToArray());
        }

        private static Dictionary<string, string> GetPostData(HttpRequest request)
        {
            if (request.RequestType != "POST" || request.Form.AllKeys.Length == 0)
                return null;

            var data = new Dictionary<string, string>();
            foreach (var item in request.Form.AllKeys)
            {
                data.Add(item, request.Form[item]);
            }

            return data;
        }
    }
}
