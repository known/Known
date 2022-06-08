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
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Routing;
using Known.Core;

namespace Known.Web
{
    class ActionContext
    {
        internal ActionContext(RequestContext requestContext, HttpContext httpContext)
        {
            HttpContext = httpContext;
            AppPath = httpContext.Request.ApplicationPath;

            var url = httpContext.Request.Url.LocalPath;
            if (AppPath != "/")
            {
                url = url.Replace(AppPath, "");
            }
            Url = url.Trim('/');

            var app = requestContext.RouteData.Values["app"];
            if (app != null)
                App = app.ToString();

            var service = requestContext.RouteData.Values["service"];
            if (service != null)
                Service = service.ToString();

            var action = requestContext.RouteData.Values["action"];
            if (action != null)
                Action = action.ToString();
        }

        internal HttpContext HttpContext { get; }
        internal string AppPath { get; }
        internal string Url { get; }
        internal string App { get; }
        internal string Service { get; }
        internal string Action { get; }
    }

    class ServiceHandler
    {
        internal static void Execute(HttpContext context)
        {
            var url = context.Request.Url.LocalPath.Trim('/');
            if (url.StartsWith("static/"))
                return;

            var referer = context.Request.UrlReferrer;
            var refererUrl = referer == null ? string.Empty : referer.LocalPath.Trim('/');
            if (!Config.IsInstalled && url != "install" && refererUrl != "install")
            {
                ActionResult.Redirect(context, "~/install");
                return;
            }

            if (string.IsNullOrEmpty(url))
                url = "index";

            if (ViewEngine.ExistsView(url, out _))
            {
                ActionResult.View(context, url);
                return;
            }

            var action = ServiceHelper.GetAction(url);
            if (action == null || action.Service == null || action.Method == null)
            {
                ActionResult.Content(context, $"{url}不存在！");
                return;
            }

            if (!action.IsAnonymous)
            {
                if (!ValidateAction(context))
                    return;
            }

            InvokeAction(context, action);
        }

        internal static void Execute(ActionContext context)
        {
            var url = context.Url;
            if (string.IsNullOrEmpty(url))
                url = "index";

            if (ViewEngine.ExistsView(url, out _))
            {
                ActionResult.View(context.HttpContext, url);
                return;
            }

            if (url == "index")
            {
                var html = WebUtils.GetIndexHtml(context.HttpContext.Request, "/static");
                ActionResult.Content(context.HttpContext, html, MimeTypes.TextHtml);
                return;
            }

            var action = ServiceHelper.GetAction(context);
            if (action == null || action.Service == null || action.Method == null)
            {
                ActionResult.Content(context.HttpContext, $"{url}不存在！");
                return;
            }

            if (!action.IsAnonymous)
            {
                if (!ValidateAction(context.HttpContext))
                    return;
            }

            InvokeAction(context.HttpContext, action);
        }

        private static bool ValidateAction(HttpContext context)
        {
            var user = UserHelper.GetUser(out string error);
            if (user == null)
            {
                ActionResult.Json(context, new { timeout = true, msg = error ?? "登录超时！" });
                return false;
            }

            var referrer = context.Request.UrlReferrer;
            var path = context.Request.Url.PathAndQuery;
            if (path.StartsWith("/Home/Style") || path.StartsWith("/Home/Script"))
                path = referrer.PathAndQuery;

            var menus = UserHelper.GetMenus();
            if (menus == null)
                menus = new List<MenuInfo>();
            menus.Add(new MenuInfo { Url = "/" });

            if (!menus.Exists(m => !string.IsNullOrEmpty(m.Url) && path.StartsWith(m.Url)))
            {
                ActionResult.Json(context, new { error = true, type = "403" });
                return false;
            }

            return true;
        }

        private static void InvokeAction(HttpContext context, ActionInfo action)
        {
            SetRequestData(action, context.Request);
            var service = Activator.CreateInstance(action.Service.Type) as ServiceBase;
            service.PrototypeName = action.PrototypeName;
            var ar = new ActionResult(context, action);
            var paras = action.Parameters;
            if (paras != null && paras.Length == 1 && paras[0].ParameterType == typeof(PagingCriteria))
            {
                var user = UserHelper.GetUser(out _);
                ar.Query(service, action.Method, user);
            }
            else
            {
                var data = InvokeAction(context.Request.Files, service, action);
                if (data != null)
                {
                    if (data is Result result)
                        ar.Validate(result);
                    else if (data is ApiFile file)
                        ar.File(file.Bytes, file.FileName, file.ContentType);
                    else if (data is string content)
                        ar.Content(content);
                    else
                        ar.Json(data);
                }
            }
        }

        private static object InvokeAction(HttpFileCollection files, ServiceBase service, ActionInfo action)
        {
            var method = action.Method;
            var parameters = action.Parameters;
            if (parameters == null || parameters.Length == 0)
                return method.Invoke(service, null);

            var datas = action.Datas;
            var paras = new List<object>();
            foreach (var item in parameters)
            {
                if (item.ParameterType == typeof(HttpFileCollection))
                {
                    paras.Add(files);
                }
                else
                {
                    var value = datas.ContainsKey(item.Name)
                              ? datas[item.Name]
                              : string.Empty;
                    if (item.ParameterType == typeof(string))
                        paras.Add(value);
                    else if (item.ParameterType.IsClass)
                        paras.Add(Utils.FromJson(item.ParameterType, value));
                    else
                        paras.Add(Utils.ConvertTo(item.ParameterType, value, null));
                }
            }

            return method.Invoke(service, paras.ToArray());
        }

        private static void SetRequestData(ActionInfo action, HttpRequest request)
        {
            var parameters = action.Parameters;
            if (parameters == null || parameters.Length == 0)
                return;

            var queries = HttpUtility.ParseQueryString(request.Url.Query);
            if (queries != null && queries.Count > 0)
            {
                foreach (string key in queries.Keys)
                {
                    action.Datas[key] = queries[key];
                }
            }

            if (request.HttpMethod == "POST")
            {
                if (request.Form != null && request.Form.Count > 0)
                {
                    foreach (string key in request.Form.Keys)
                    {
                        action.Datas[key] = request.Form[key];
                    }
                }
                else
                {
                    request.InputStream.Position = 0;
                    using (var stream = new StreamReader(request.InputStream))
                    {
                        var json = stream.ReadToEnd();
                        var datas = Utils.FromJson<Dictionary<string, string>>(json);
                        if (datas != null && datas.Count > 0)
                        {
                            foreach (var item in datas)
                            {
                                action.Datas[item.Key] = item.Value;
                            }
                        }
                    }
                }
            }
        }
    }
}
#endif