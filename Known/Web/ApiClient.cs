using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using Known.Extensions;
using Known.Web.Extensions;

namespace Known.Web
{
    public class ApiClient
    {
        private readonly int lengthThreshold = 5000;
        private readonly CompressionMethod defaultCompressionMethod = CompressionMethod.GZip;
        private readonly AuthenticationHeaderValue authorization;

        public ApiClient() { }

        public ApiClient(string token)
        {
            authorization = new AuthenticationHeaderValue("Token", token);
        }

        public ApiClient(string account, string password)
        {
            var authParameter = string.Format("{0}:{1}", account, password);
            var bytes = Encoding.Default.GetBytes(authParameter);
            var parameter = Convert.ToBase64String(bytes);
            authorization = new AuthenticationHeaderValue("Basic", parameter);
        }

        public string Get(string url, dynamic param = null)
        {
            return Request("GET", url, param);
        }

        public T Get<T>(string url, dynamic param = null)
        {
            var json = Get(url, param) as string;
            json = ConvertJson<T>(json);
            return json.FromJson<T>();
        }

        public string Post(string url, dynamic param = null)
        {
            return Request("POST", url, param);
        }

        public T Post<T>(string url, dynamic param = null)
        {
            var json = Post(url, param) as string;
            json = ConvertJson<T>(json);
            return json.FromJson<T>();
        }

        private static string ConvertJson<T>(string json)
        {
            if (typeof(T) != typeof(ApiResult))
            {
                var result = json.FromJson<ApiResult>();
                if (result.Status == 0)
                {
                    json = SerializeExtension.ToJson(result.Data);
                }
            }

            return json;
        }

        private string Request(string method, string url, dynamic param, CompressionMethod compressionMethod = CompressionMethod.Automatic)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("url");

            var query = GetQueryString(method, param);
            url = GetApiFullUrl(url) + "?" + query;
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            using (var httpClient = new HttpClient(handler))
            {
                if (authorization != null)
                    httpClient.DefaultRequestHeaders.Authorization = authorization;

                HttpResponseMessage response;
                if (method == "GET")
                {
                    response = httpClient.GetAsync(url).Result;
                }
                else
                {
                    var meta = new MediaTypeWithQualityHeaderValue("application/json");
                    httpClient.DefaultRequestHeaders.Accept.Add(meta);
                    var data = SerializeExtension.ToJson(param);
                    HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");
                    if (compressionMethod == CompressionMethod.Automatic && data.Length >= lengthThreshold)
                        content = new CompressedContent(content, defaultCompressionMethod);
                    else if (compressionMethod == CompressionMethod.GZip || compressionMethod == CompressionMethod.Deflate)
                        content = new CompressedContent(content, compressionMethod);

                    response = httpClient.PostAsync(url, content).Result;
                }

                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsStringAsync().Result;
            }
        }

        private static string GetQueryString(string method, dynamic param)
        {
            var dic = new Dictionary<string, object>();
            if (param != null)
            {
                if (method == "GET")
                {
                    var properties = param.GetType().GetProperties();
                    foreach (var item in properties)
                    {
                        dic[item.Name] = item.GetValue(param);
                    }
                }
                else
                {
                    dic["body"] = SerializeExtension.ToJson(param);
                }
            }

            dic["timestamp"] = DateTime.Now.ToTimestamp();
            dic["nonce"] = Utils.NewGuid;
            dic["sign"] = dic.ToMd5Signature();

            var values = dic.Where(d => d.Key != "body")
                            .Select(d => string.Format("{0}={1}", d.Key, HttpUtility.UrlEncode(d.Value.ToString())));
            return string.Join("&", values);
        }

        private static string GetApiFullUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return string.Empty;

            if (url.StartsWith("http"))
                return url;

            var baseUrl = Config.AppSetting("ApiBaseUrl");
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                if (HttpContext.Current != null)
                    baseUrl = HttpContext.Current.Request.GetHostName();
            }

            return baseUrl + url;
        }
    }
}
