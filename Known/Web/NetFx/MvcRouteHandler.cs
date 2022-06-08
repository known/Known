#if !NET6_0
using System.Web;
using System.Web.Routing;

namespace Known.Web
{
    class MvcRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new MvcHandler(requestContext);
        }
    }
}
#endif