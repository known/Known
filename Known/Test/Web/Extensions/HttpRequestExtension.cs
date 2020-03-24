using System.Web;

namespace Known.Web.Extensions
{
    /// <summary>
    /// 请求扩展类。
    /// </summary>
    public static class HttpRequestExtension
    {
        /// <summary>
        /// 判断Http请求是否是Ajax请求。
        /// </summary>
        /// <param name="request">请求对象。</param>
        /// <returns>是否是Ajax请求。</returns>
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            var ajax = request.Headers.Get("X-Requested-With");
            return ajax == "XMLHttpRequest";
        }

        /// <summary>
        /// 获取指定键及类型的请求值，统一获取查询和窗体的变量。
        /// </summary>
        /// <typeparam name="T">值类型。</typeparam>
        /// <param name="request">请求对象。</param>
        /// <param name="name">键名。</param>
        /// <param name="defaultValue">为空时的默认值。</param>
        /// <returns>请求值。</returns>
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
        /// 获取客户端上次请求的地址。
        /// </summary>
        /// <param name="request">请求对象。</param>
        /// <returns>请求地址。</returns>
        public static string GetUrlReferrer(this HttpRequest request)
        {
            if (request.UrlReferrer != null)
                return request.UrlReferrer.ToString();

            return request.GetHostName();
        }

        /// <summary>
        /// 获取请求的完整路径。
        /// </summary>
        /// <param name="request">请求对象。</param>
        /// <param name="virtualPath">虚拟路径。</param>
        /// <returns>完整路径。</returns>
        public static string GetFullUrl(this HttpRequest request, string virtualPath)
        {
            var hostName = request.GetHostName();
            var absoluteUrl = VirtualPathUtility.ToAbsolute(virtualPath).ToLower();
            return hostName + absoluteUrl;
        }

        /// <summary>
        /// 添加请求参数片段。
        /// </summary>
        /// <param name="request">请求对象。</param>
        /// <param name="fragment">参数片段。</param>
        /// <returns>完整的请求地址。</returns>
        public static string AddFragment(this HttpRequest request, string fragment)
        {
            return WebUtils.AddUrlFragment(request.RawUrl, fragment);
        }

        /// <summary>
        /// 获取主机协议和域名。
        /// </summary>
        /// <param name="request">请求对象。</param>
        /// <returns>主机协议和域名。</returns>
        public static string GetHostName(this HttpRequest request)
        {
            return WebUtils.GetHostName(request.Url);
        }

        /// <summary>
        /// 获取操作系统名称。
        /// </summary>
        /// <param name="request">请求对象。</param>
        /// <returns>操作系统名称。</returns>
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
        /// 获取IP地址。
        /// </summary>
        /// <param name="request">请求对象。</param>
        /// <returns>IP地址。</returns>
        public static string GetIPAddress(this HttpRequest request)
        {
            var result = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
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

        /// <summary>
        /// 获取IP地址所属地名。
        /// </summary>
        /// <param name="request">请求对象。</param>
        /// <returns>IP地址所属地名。</returns>
        public static string GetIPAddressName(this HttpRequest request)
        {
            var ipAddress = request.GetIPAddress();
            return Utils.GetIPAddressName(ipAddress);
        }
    }
}
