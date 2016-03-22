using Known.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Known.Web.Extensions
{
    public static class HttpRequestExtension
    {
        public static T Get<T>(this HttpRequest request, string name)
        {
            return request.Get<T>(name, default(T));
        }

        public static T Get<T>(this HttpRequest request, string name, T defaultValue)
        {
            var value = request.QueryString[name];
            if (!string.IsNullOrEmpty(value))
                return value.To<T>();

            var formValue = request.Form[name];
            if (!string.IsNullOrEmpty(formValue))
                return formValue.To<T>();

            return defaultValue;
        }

        public static bool IsPost(this HttpRequest request)
        {
            return request.HttpMethod.Equals("POST");
        }

        public static bool IsGet(this HttpRequest request)
        {
            return request.HttpMethod.Equals("GET");
        }

        public static bool IsCrossSitePost(this HttpRequest request)
        {
            if (request.IsPost())
            {
                var urlReferrer = Convert.ToString(request.UrlReferrer);
                if (urlReferrer.Length < 7)
                {
                    return true;
                }
                Uri u = new Uri(urlReferrer);
                return u.Host != request.Url.Host;
            }
            return false;
        }

        public static string GetIPAddress(this HttpRequest request)
        {
            string result = String.Empty;
            result = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (null == result || result == String.Empty)
            {
                result = request.ServerVariables["REMOTE_ADDR"];
            }
            if (null == result || result == String.Empty)
            {
                result = request.UserHostAddress;
            }
            if (null == result || result == String.Empty || !result.IsIPAddress())
            {
                return "0.0.0.0";
            }
            return result;
        }
    }
}
