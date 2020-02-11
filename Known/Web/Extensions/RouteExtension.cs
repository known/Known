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
    }
}
