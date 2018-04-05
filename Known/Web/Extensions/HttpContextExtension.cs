using System.Web;
using System.Web.Routing;

namespace Known.Web.Extensions
{
    /// <summary>
    /// Http上下文扩展类。
    /// </summary>
    public static class HttpContextExtension
    {
        /// <summary>
        /// 根据Http上下文获取已定义路由匹配的请求上下文。
        /// </summary>
        /// <param name="context">Http上下文。</param>
        /// <returns>已定义路由匹配的请求上下文。</returns>
        public static RequestContext GetRequestContext(this HttpContext context)
        {
            var httpContext = new HttpContextWrapper(HttpContext.Current);
            var routeData = RouteTable.Routes.GetRouteData(httpContext);
            return new RequestContext(httpContext, routeData);
        }
    }
}
