using System.Linq;
using System.Net.Http;
using System.Web;
using Known.Web.Extensions;

namespace Known.Web.Api.Extensions
{
    /// <summary>
    /// Http请求消息扩展类。
    /// </summary>
    public static class HttpRequestMessageExtension
    {
        /// <summary>
        /// 获取查询参数值。
        /// </summary>
        /// <param name="request">Http请求消息。</param>
        /// <param name="name">查询参数名称。</param>
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
        /// 获取完整的资源路径。
        /// </summary>
        /// <param name="request">Http请求消息。</param>
        /// <param name="virtualPath">资源虚拟路径。</param>
        /// <returns>完整的资源路径。</returns>
        public static string GetContentUrl(this HttpRequestMessage request, string virtualPath)
        {
            return request.GetFullUrl(virtualPath);
        }
    }
}