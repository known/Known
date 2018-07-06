using System.Web;

namespace Known.Web.Extensions
{
    public static class HttpRequestBaseExtension
    {
        public static T Get<T>(this HttpRequestBase request, string name, T defaultValue = default(T))
        {
            var value = request.QueryString[name];
            if (!string.IsNullOrEmpty(value))
                return Utils.ConvertTo(value, defaultValue);

            var formValue = request.Form[name];
            if (!string.IsNullOrEmpty(formValue))
                return Utils.ConvertTo(formValue, defaultValue);

            return defaultValue;
        }

        public static string GetUrlReferrer(this HttpRequestBase request)
        {
            if (request.UrlReferrer != null)
                return request.UrlReferrer.ToString();

            return request.GetHostName();
        }

        public static string GetFullUrl(this HttpRequestBase request, string virtualPath)
        {
            var hostName = request.GetHostName();
            var absoluteUrl = VirtualPathUtility.ToAbsolute(virtualPath).ToLower();
            return hostName + absoluteUrl;
        }

        public static string AddFragment(this HttpRequestBase request, string fragment)
        {
            return WebUtils.AddUrlFragment(request.RawUrl, fragment);
        }

        public static string GetHostName(this HttpRequestBase request)
        {
            return WebUtils.GetHostName(request.Url);
        }

        public static string GetOSName(this HttpRequestBase request)
        {
            var userAgent = request.UserAgent;
            var osName = WebUtils.GetOSName(userAgent);
            if (string.IsNullOrEmpty(osName))
            {
                osName = request.Browser.Platform;
            }
            return osName;
        }

        public static string GetIPAddress(this HttpRequestBase request)
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
