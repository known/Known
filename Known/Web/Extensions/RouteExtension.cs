using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Known.Web.Extensions
{
    /// <summary>
    /// 路由扩展类。
    /// </summary>
    public static class RouteExtension
    {
        /// <summary>
        /// 获取请求路径。
        /// </summary>
        /// <param name="route">路由信息。</param>
        /// <returns>请求路径。</returns>
        public static string GetRequestPath(this RouteData route)
        {
            var areaName = route.DataTokens["area"];
            var controllerName = route.Values["controller"];
            var actionName = route.Values["action"];
            var requestPath = string.Format("/{0}/{1}", controllerName, actionName);
            if (areaName != null)
                requestPath = string.Format("/{0}{1}", areaName, requestPath);
            return requestPath;
        }

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
    }
}
