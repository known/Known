using System.Linq;
using System.Net.Http;
using System.Web;

namespace Known.Web.Extensions
{
    /// <summary>
    /// Http请求消息扩展类。
    /// </summary>
    public static class HttpRequestMessageExtension
    {
        private const string HttpContext = "MS_HttpContext";
        private const string RemoteEndpointMessage = "System.ServiceModel.Channels.RemoteEndpointMessageProperty";
        private const string OwinContext = "MS_OwinContext";

        /// <summary>
        /// 根据虚拟路径获取完整的请求URL。
        /// </summary>
        /// <param name="request">Http请求消息。</param>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <returns>完整的请求URL。</returns>
        public static string GetFullUrl(this HttpRequestMessage request, string virtualPath)
        {
            var hostName = WebUtils.GetHostName(request.RequestUri);
            var absoluteUrl = VirtualPathUtility.ToAbsolute(virtualPath).ToLower();
            return hostName + absoluteUrl;
        }

        /// <summary>
        /// 获取客户端IP地址。
        /// </summary>
        /// <param name="request">Http请求消息。</param>
        /// <returns>客户端IP地址。</returns>
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

        /// <summary>
        /// 根据标头参数名称获取请求标头数据。
        /// </summary>
        /// <param name="request">Http请求消息。</param>
        /// <param name="name">标头参数名称。</param>
        /// <returns>请求标头数据。</returns>
        public static string GetHeaderValue(this HttpRequestMessage request, string name)
        {
            if (!request.Headers.Contains(name))
                return null;

            var value = request.Headers.GetValues(name).First();
            return HttpUtility.UrlDecode(value).Trim();
        }

        /// <summary>
        /// 根据属性Key获取请求属性值。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request">Http请求消息。</param>
        /// <param name="key">属性Key。</param>
        /// <returns>请求属性值。</returns>
        public static T GetPropertyValue<T>(this HttpRequestMessage request, string key)
        {
            if (!request.Properties.ContainsKey(key))
                return default(T);

            return (T)request.Properties[key];
        }
    }
}
