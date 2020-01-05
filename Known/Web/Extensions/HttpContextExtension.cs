using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Known.Web.Extensions
{
    /// <summary>
    /// Http上下文扩展类。
    /// </summary>
    public static class HttpContextExtension
    {
        /// <summary>
        /// 获取请求上下文对象。
        /// </summary>
        /// <param name="context">Http上下文对象。</param>
        /// <returns>请求上下文对象。</returns>
        public static RequestContext GetRequestContext(this HttpContext context)
        {
            var httpContext = new HttpContextWrapper(context);
            var routeData = RouteTable.Routes.GetRouteData(httpContext);
            return new RequestContext(httpContext, routeData);
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
    }
}
