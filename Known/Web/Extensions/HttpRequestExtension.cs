using System.Web;

namespace Known.Web.Extensions
{
    /// <summary>
    /// Http请求扩展类。
    /// </summary>
    public static class HttpRequestExtension
    {
        /// <summary>
        /// 根据请求参数名获取指定类型的参数值。
        /// </summary>
        /// <typeparam name="T">参数值的类型。</typeparam>
        /// <param name="request">Http请求。</param>
        /// <param name="name">请求参数名。</param>
        /// <param name="defaultValue">请求默认值。</param>
        /// <returns>指定类型的参数值。</returns>
        public static T Get<T>(this HttpRequest request, string name, T defaultValue = default(T))
        {
            var value = request.QueryString[name];
            if (!string.IsNullOrEmpty(value))
                return Utils.ConvertTo(value, defaultValue);

            var formValue = request.Form[name];
            if (!string.IsNullOrEmpty(formValue))
                return Utils.ConvertTo(formValue, defaultValue);

            return defaultValue;
        }

        /// <summary>
        /// 获取当前请求的上一个请求的URL。
        /// </summary>
        /// <param name="request">Http请求。</param>
        /// <returns>上一个请求的URL。</returns>
        public static string GetUrlReferrer(this HttpRequest request)
        {
            if (request.UrlReferrer != null)
                return request.UrlReferrer.ToString();

            return request.GetHostName();
        }

        /// <summary>
        /// 根据虚拟路径获取完整的URL。
        /// </summary>
        /// <param name="request">Http请求。</param>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <returns>完整的URL。</returns>
        public static string GetFullUrl(this HttpRequest request, string virtualPath)
        {
            var hostName = request.GetHostName();
            var absoluteUrl = VirtualPathUtility.ToAbsolute(virtualPath).ToLower();
            return hostName + absoluteUrl;
        }

        /// <summary>
        /// 添加URL参数片段。
        /// </summary>
        /// <param name="request">Http请求。</param>
        /// <param name="fragment">参数片段。</param>
        /// <returns>添加后的完整URL。</returns>
        public static string AddFragment(this HttpRequest request, string fragment)
        {
            return WebUtils.AddUrlFragment(request.RawUrl, fragment);
        }

        /// <summary>
        /// 获取远程主机协议及域名。
        /// </summary>
        /// <param name="request">Http请求。</param>
        /// <returns>远程主机协议及域名。</returns>
        public static string GetHostName(this HttpRequest request)
        {
            return WebUtils.GetHostName(request.Url);
        }

        /// <summary>
        /// 获取客户端操作系统名。
        /// </summary>
        /// <param name="request">Http请求。</param>
        /// <returns>客户端操作系统名。</returns>
        public static string GetOSName(this HttpRequest request)
        {
            var userAgent = request.UserAgent;
            var osName = WebUtils.GetOSName(userAgent);
            if (string.IsNullOrEmpty(osName))
            {
                osName = request.Browser.Platform;
            }
            return osName;
        }

        /// <summary>
        /// 获取客户端IP地址。
        /// </summary>
        /// <param name="request">Http请求。</param>
        /// <returns>客户端IP地址。</returns>
        public static string GetIPAddress(this HttpRequest request)
        {
            var result = string.Empty;
            result = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(result))
            {
                result = request.ServerVariables["REMOTE_ADDR"];
            }
            if (string.IsNullOrEmpty(result))
            {
                result = request.UserHostAddress;
            }
            if (string.IsNullOrEmpty(result) || result == "::1")
            {
                return "127.0.0.1";
            }
            return result;
        }
    }
}
