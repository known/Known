using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Known.Web.Extensions
{
    /// <summary>
    /// Html扩展类。
    /// </summary>
    public static class HtmlExtension
    {
        /// <summary>
        /// 获取请求地址。
        /// </summary>
        /// <param name="context">Http上下文。</param>
        /// <param name="actionName">方法名。</param>
        /// <param name="controllerName">控制器名。</param>
        /// <param name="routeValues">路由数据。</param>
        /// <returns>请求地址。</returns>
        public static string GetAction(this HttpContext context, string actionName, string controllerName, object routeValues)
        {
            var httpContext = new HttpContextWrapper(HttpContext.Current);
            var routeData = RouteTable.Routes.GetRouteData(httpContext);
            var requestContext = new RequestContext(httpContext, routeData);
            var helper = new UrlHelper(requestContext);
            return helper.Action(actionName, controllerName, routeValues);
        }

        /// <summary>
        /// 获取请求地址。
        /// </summary>
        /// <param name="context">Http上下文。</param>
        /// <param name="actionName">方法名。</param>
        /// <param name="controllerName">控制器名。</param>
        /// <param name="routeValues">路由数据。</param>
        /// <returns>请求地址。</returns>
        public static string GetAction(this RequestContext context, string actionName, string controllerName, object routeValues = null)
        {
            var helper = new UrlHelper(context);
            return helper.Action(actionName, controllerName, routeValues);
        }

        /// <summary>
        /// 判断操作方法是否使用指定类型的特性。
        /// </summary>
        /// <typeparam name="T">特性类型。</typeparam>
        /// <param name="context">执行操作上下文对象。</param>
        /// <returns>是否返回 True，否则返回 False。</returns>
        public static bool IsUseAttributeOf<T>(this AuthorizationContext context) where T : Attribute
        {
            return context.ActionDescriptor.GetCustomAttributes(typeof(T), true).Length > 0 ||
                   context.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(T), true).Length > 0;
        }

        /// <summary>
        /// 获取带日期版本的样式文件路径。
        /// </summary>
        /// <param name="helper">Html帮助者。</param>
        /// <param name="path">文件路径。</param>
        /// <returns>带日期版本的文件路径。</returns>
        public static IHtmlString Style(this HtmlHelper helper, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return new HtmlString("");

            var format = "<link rel=\"stylesheet\" href=\"/{0}\">";
            var html = GetHtmlString(helper, format, path, ".css");
            return new HtmlString(html);
        }

        /// <summary>
        /// 获取带日期版本的脚本文件路径。
        /// </summary>
        /// <param name="helper">Html帮助者。</param>
        /// <param name="path">文件路径。</param>
        /// <returns>带日期版本的文件路径。</returns>
        public static IHtmlString Script(this HtmlHelper helper, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return new HtmlString("");

            var format = "<script src=\"/{0}\"></script>";
            var html = GetHtmlString(helper, format, path, ".js");
            return new HtmlString(html);
        }

        private static string GetHtmlString(HtmlHelper helper, string format, string path, string extName)
        {
            var random = string.Empty;
            var httpContext = helper.ViewContext.RequestContext.HttpContext;
            var rootPath = httpContext.Server.MapPath("~/");

            if (path.EndsWith(extName))
            {
                var fileName = httpContext.Server.MapPath(path);
                var file = new FileInfo(fileName);
                random = file.LastWriteTime.ToString("yyMMddHHmmss");
                var filePath = fileName.Replace(rootPath, "").Replace("\\", "/");
                return string.Format(format, $"{filePath}?r={random}");
            }

            var dirPath = httpContext.Server.MapPath(path);
            var dir = new DirectoryInfo(dirPath);
            if (!dir.Exists)
                return string.Empty;

            var files = dir.GetFiles($"*{extName}");
            if (files == null || files.Length == 0)
                return string.Empty;

            var paths = new List<string>();
            random = files.Max(f => f.LastWriteTime).ToString("yyMMddHHmmss");
            foreach (var file in files)
            {
                var filePath = file.FullName.Replace(rootPath, "").Replace("\\", "/");
                paths.Add(string.Format(format, $"{filePath}?r={random}"));
            }

            return string.Join(Environment.NewLine, paths);
        }
    }
}
