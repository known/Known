#if !NET6_0
using System.Web.Routing;

namespace Known.Web
{
    class MvcRoute : Route
    {
        public MvcRoute(string url) : base(url, new MvcRouteHandler())
        {
        }

        public MvcRoute(string url, RouteValueDictionary defaults) : base(url, defaults, new MvcRouteHandler())
        {
        }
    }
}
#endif