using System.Linq;
using System.Net.Http;
using System.Web;

namespace Known.Web.Extensions
{
    /// <summary>
    /// 请求消息扩展类。
    /// </summary>
    public static class HttpRequestMessageExtension
    {
        private const string HttpContext = "MS_HttpContext";
        private const string RemoteEndpointMessage = "System.ServiceModel.Channels.RemoteEndpointMessageProperty";
        private const string OwinContext = "MS_OwinContext";

        /// <summary>
        /// 获取请求的查询参数。
        /// </summary>
        /// <param name="request">请求消息对象。</param>
        /// <param name="name">查询参数名。</param>
        /// <returns>查询参数值。</returns>
        public static string GetQueryValue(this HttpRequestMessage request, string name)
        {
            var pairs = request.GetQueryNameValuePairs()
                               .Where(e => e.Key.ToLower() == name.ToLower())
                               .ToList();
            if (pairs.Count > 0)
            {
                var value = pairs.First().Value;
                return HttpUtility.UrlDecode(value).Trim();
            }
            return null;
        }

        /// <summary>
        /// 获取请求虚拟路径的完整路径。
        /// </summary>
        /// <param name="request">请求消息对象。</param>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <returns>完整路径。</returns>
        public static string GetContentUrl(this HttpRequestMessage request, string virtualPath)
        {
            var hostName = WebUtils.GetHostName(request.RequestUri);
            var absoluteUrl = VirtualPathUtility.ToAbsolute(virtualPath).ToLower();
            return hostName + absoluteUrl;
        }

        /// <summary>
        /// 获取请求消息的客户端IP。
        /// </summary>
        /// <param name="request">请求消息对象。</param>
        /// <returns>客户端IP。</returns>
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
        /// 获取指定名称的请求头信息。
        /// </summary>
        /// <param name="request">请求消息对象。</param>
        /// <param name="name">头名称。</param>
        /// <returns>头信息。</returns>
        public static string GetHeaderValue(this HttpRequestMessage request, string name)
        {
            if (!request.Headers.Contains(name))
                return null;

            var value = request.Headers.GetValues(name).First();
            return HttpUtility.UrlDecode(value).Trim();
        }

        /// <summary>
        /// 获取指定键的请求消息属性。
        /// </summary>
        /// <typeparam name="T">属性类型。</typeparam>
        /// <param name="request">请求消息对象。</param>
        /// <param name="key">请求键。</param>
        /// <returns>属性对象。</returns>
        public static T GetPropertyValue<T>(this HttpRequestMessage request, string key)
        {
            if (!request.Properties.ContainsKey(key))
                return default;

            return (T)request.Properties[key];
        }
    }
}
