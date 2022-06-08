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
using System.Reflection;
using System.Web;

namespace Known.Web
{
    class ActionResult
    {
        private readonly HttpContext context;
        private readonly ActionInfo action;

        internal ActionResult(HttpContext context, ActionInfo action)
        {
            this.context = context;
            this.action = action;
        }

        internal static void View(HttpContext context, string url)
        {
            var host = WebAppContext.GetHost(context);
            var user = UserHelper.GetUser(out _);
            var ctx = new ViewContext
            {
                App = ViewAppInfo.Create(host, user),
                Assembly = Assembly.GetExecutingAssembly(),
                Url = url,
                HttpContext = context,
                Minify = Container.Resolve<IMinify>()
            };
            var content = ViewEngine.GetView(ctx);
            context.Response.ContentType = MimeTypes.TextHtml;
            context.Response.Write(content);
        }

        internal static void Redirect(HttpContext context, string url)
        {
            context.Response.Redirect(url, true);
        }

        internal static void Content(HttpContext context, string content, string contentType = null)
        {
            context.Response.ContentType = contentType ?? MimeTypes.TextPlain;
            context.Response.Write(content);
        }

        internal static void Json(HttpContext context, object value)
        {
            var json = Utils.ToJson(value);
            Content(context, json, MimeTypes.ApplicationJson);
        }

        internal void View()
        {
            var context = GetViewContext();
            var content = ViewEngine.GetView(context);
            Content(content, MimeTypes.TextHtml);
        }

        internal void Partial(string viewName)
        {
            var context = GetViewContext(viewName);
            var content = ViewEngine.GetView(context);
            Content(content, MimeTypes.TextHtml);
        }

        internal void Content(string content, string contentType = null)
        {
            Content(context, content, contentType);
        }

        internal void Json(object value)
        {
            Json(context, value);
        }

        internal void Query(object service, MethodInfo action, UserInfo user)
        {
            var request = context.Request;
            var criteria = new PagingCriteria(user.CompNo)
            {
                load = Utils.ConvertTo<int>(request.Form["load"]),
                query = request.Form["query"],
                page = Utils.ConvertTo<int>(request.Form["page"]),
                limit = Utils.ConvertTo(request.Form["limit"], 10),
                field = request.Form["field"],
                order = request.Form["order"]
            };
            var result = action.Invoke(service, new object[] { criteria });
            Json(result);
        }

        internal void File(byte[] bytes, string fileName, string contentType = null)
        {
            if (bytes == null || bytes.Length == 0)
            {
                Error("文件内容为空！");
                return;
            }

            if (!string.IsNullOrEmpty(fileName))
            {
                var key = Constants.KeyDownload;
                var token = context.Request.Form[key];
                context.Response.SetCookie(new HttpCookie(key, token));
                context.Response.AddHeader("Content-Disposition", $"filename={fileName}");
                
                if (fileName.ToLower().EndsWith(".pdf"))
                    contentType = MimeTypes.ApplicationPdf;
                else if (fileName.ToLower().EndsWith(".xlsx"))
                    contentType = "application/vnd.ms-excel";
            }

            context.Response.ContentType = contentType ?? MimeTypes.ApplicationOctetStream;
            context.Response.BinaryWrite(bytes);
        }

        internal void Validate(Result result)
        {
            if (result == null)
                Error("返回数据出错！");
            else if (!result.IsValid)
                Error(result.Message, result.Data);
            else
                Success(result.Message, result.Data);
        }

        private void Error(string message, object data = null)
        {
            Json(new { IsValid = false, Message = message, Data = data });
        }

        private void Success(string message, object data = null)
        {
            Json(new { IsValid = true, Message = message, Data = data });
        }

        private ViewContext GetViewContext(string partialName = null)
        {
            var host = WebAppContext.GetHost(context);
            var user = UserHelper.GetUser(out _);
            var ctx = new ViewContext
            {
                App = ViewAppInfo.Create(host, user),
                //LayoutAssembly = typeof(ControllerBase).Assembly,
                Assembly = action.Service.Type.Assembly,
                HttpContext = context,
                Minify = Container.Resolve<IMinify>(),
                PartialName = partialName
            };

            ctx.Controller = action.Service.Name;
            ctx.Action = action.Name;
            if (action.Datas.ContainsKey("id"))
            {
                ctx.ParamId = action.Datas["id"];
            }
            return ctx;
        }
    }
}
#endif