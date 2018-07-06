using System.Linq;
using System.Net.Http;
using System.Web;

namespace Known.Web.Extensions
{
    public static class HttpRequestMessageExtension
    {
        private const string HttpContext = "MS_HttpContext";
        private const string RemoteEndpointMessage = "System.ServiceModel.Channels.RemoteEndpointMessageProperty";
        private const string OwinContext = "MS_OwinContext";

        public static string GetFullUrl(this HttpRequestMessage request, string virtualPath)
        {
            var hostName = WebUtils.GetHostName(request.RequestUri);
            var absoluteUrl = VirtualPathUtility.ToAbsolute(virtualPath).ToLower();
            return hostName + absoluteUrl;
        }

        public static string GetClientIP(this HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey(HttpContext))
            {
                dynamic httpContext = request.Properties[HttpContext];
                if (httpContext != null)
                    return httpContext.Request.UserHostAddress;
            }

            if (request.Properties.ContainsKey(RemoteEndpointMessage))
            {
                dynamic remoteEndpoint = request.Properties[RemoteEndpointMessage];
                if (remoteEndpoint != null)
                    return remoteEndpoint.Address;
            }

            if (request.Properties.ContainsKey(OwinContext))
            {
                dynamic owinContext = request.Properties[OwinContext];
                if (owinContext != null)
                    return owinContext.Request.RemoteIpAddress;
            }

            return string.Empty;
        }

        public static string GetHeaderValue(this HttpRequestMessage request, string name)
        {
            if (!request.Headers.Contains(name))
                return null;

            var value = request.Headers.GetValues(name).First();
            return HttpUtility.UrlDecode(value).Trim();
        }

        public static T GetPropertyValue<T>(this HttpRequestMessage request, string key)
        {
            if (!request.Properties.ContainsKey(key))
                return default(T);

            return (T)request.Properties[key];
        }
    }
}
