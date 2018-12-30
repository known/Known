using System.Linq;
using System.Net.Http;
using System.Web;
using Known.Web.Extensions;

namespace Known.WebApi.Extensions
{
    public static class HttpRequestMessageExtension
    {
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

        public static string GetContentUrl(this HttpRequestMessage request, string virtualPath)
        {
            return request.GetFullUrl(virtualPath);
        }
    }
}