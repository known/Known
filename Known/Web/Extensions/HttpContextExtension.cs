using System.Web;
using System.Web.Routing;

namespace Known.Web.Extensions
{
    public static class HttpContextExtension
    {
        public static RequestContext GetRequestContext(this HttpContext context)
        {
            var httpContext = new HttpContextWrapper(HttpContext.Current);
            var routeData = RouteTable.Routes.GetRouteData(httpContext);
            return new RequestContext(httpContext, routeData);
        }
    }
}
