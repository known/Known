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
    }
}
