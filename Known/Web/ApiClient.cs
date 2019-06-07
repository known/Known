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
    /// <summary>
    /// Api 客户端操作类。
    /// </summary>
    public class ApiClient
    {
        private readonly string baseUrl;
        private readonly CompressionMethod defaultCompressionMethod = CompressionMethod.GZip;
        private readonly AuthenticationHeaderValue authorization;

        /// <summary>
        /// 初始化一个 Api 客户端操作类的实例。
        /// </summary>
        public ApiClient() : this(null) { }

        /// <summary>
        /// 初始化一个 Api 客户端操作类的实例。
        /// </summary>
        /// <param name="baseUrl">Api 根路径地址。</param>
        public ApiClient(string baseUrl)
        {
            this.baseUrl = GetBaseUrl(baseUrl);
        }

        /// <summary>
        /// 初始化一个 Api 客户端操作类的实例。
        /// </summary>
        /// <param name="baseUrl">Api 根路径地址。</param>
        /// <param name="token">用户身份认证票据。</param>
        public ApiClient(string baseUrl, string token)
        {
            this.baseUrl = GetBaseUrl(baseUrl);
            this.authorization = new AuthenticationHeaderValue("Token", token);
        }

        /// <summary>
        /// 初始化一个 Api 客户端操作类的实例。
        /// </summary>
        /// <param name="baseUrl">Api 根路径地址。</param>
        /// <param name="account">用于用户身份认证的账号。</param>
        /// <param name="password">用于用户身份认证的密码。</param>
        public ApiClient(string baseUrl, string account, string password)
        {
            this.baseUrl = GetBaseUrl(baseUrl);
            var authParameter = string.Format("{0}:{1}", account, password);
            var parameter = Utils.ToBase64String(authParameter);
            this.authorization = new AuthenticationHeaderValue("Basic", parameter);
        }

        /// <summary>
        /// 获取 GET 请求数据。
        /// </summary>
        /// <param name="url">请求的绝对或相对地址。</param>
        /// <param name="param">请求的参数对象。</param>
        /// <returns>请求的结果。</returns>
        public string Get(string url, dynamic param = null)
        {
            return Request("GET", url, param);
        }

        /// <summary>
        /// 获取 GET 请求指定类型的数据。
        /// </summary>
        /// <typeparam name="T">数据的类型。</typeparam>
        /// <param name="url">请求的绝对或相对地址。</param>
        /// <param name="param">请求的参数对象。</param>
        /// <returns>请求的结果。</returns>
        public T Get<T>(string url, dynamic param = null)
        {
            var json = Get(url, param) as string;
            json = ConvertJson<T>(json);
            return json.FromJson<T>();
        }

        /// <summary>
        /// 获取 POST 请求数据。
        /// </summary>
        /// <param name="url">请求的绝对或相对地址。</param>
        /// <param name="param">请求的参数对象。</param>
        /// <returns>请求的结果。</returns>
        public string Post(string url, dynamic param = null)
        {
            return Request("POST", url, param);
        }

        /// <summary>
        /// 获取 POST 请求指定类型的数据。
        /// </summary>
        /// <typeparam name="T">数据的类型。</typeparam>
        /// <param name="url">请求的绝对或相对地址。</param>
        /// <param name="param">请求的参数对象。</param>
        /// <returns>请求的结果。</returns>
        public T Post<T>(string url, dynamic param = null)
        {
            var json = Post(url, param) as string;
            json = ConvertJson<T>(json);
            return json.FromJson<T>();
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
            using (var httpClient = new System.Net.Http.HttpClient(handler))
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
                    var meta = new MediaTypeWithQualityHeaderValue(MimeTypes.ApplicationJson);
                    httpClient.DefaultRequestHeaders.Accept.Add(meta);
                    var data = GetParamJson(param);
                    HttpContent content = new StringContent(data, Encoding.UTF8, MimeTypes.ApplicationJson);
                    if (compressionMethod == CompressionMethod.Automatic)
                        content = new CompressedContent(content, defaultCompressionMethod);
                    else if (compressionMethod == CompressionMethod.GZip || compressionMethod == CompressionMethod.Deflate)
                        content = new CompressedContent(content, compressionMethod);

                    response = httpClient.PostAsync(url, content).Result;
                }

                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsStringAsync().Result;
            }
        }

        private string GetApiFullUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return string.Empty;

            if (url.StartsWith("http"))
                return url;

            var tmpUrl = baseUrl;
            if (string.IsNullOrWhiteSpace(tmpUrl))
            {
                if (HttpContext.Current != null)
                    tmpUrl = HttpContext.Current.Request.GetHostName();
            }

            return tmpUrl + url;
        }

        private static string GetQueryString(string method, dynamic param)
        {
            var dic = new Dictionary<string, object>();
            if (method == "GET")
            {
                if (param != null)
                {
                    var properties = param.GetType().GetProperties();
                    foreach (var item in properties)
                    {
                        dic[item.Name] = item.GetValue(param);
                    }
                }
            }
            else
            {
                dic["body"] = GetParamJson(param);
            }

            dic["timestamp"] = DateTime.Now.ToTimestamp();
            dic["nonce"] = Utils.NewGuid;
            dic["sign"] = dic.ToMd5Signature();

            var values = dic.Where(d => d.Key != "body")
                            .Select(d =>
                            {
                                var value = string.Empty;
                                if (d.Value != null)
                                {
                                    value = d.Value.ToString();
                                    value = HttpUtility.UrlEncode(value);
                                }
                                return $"{d.Key}={value}";
                            });
            return string.Join("&", values);
        }

        private static string GetParamJson(dynamic param)
        {
            if (param == null)
                return string.Empty;

            return SerializeExtension.ToJson(param);
        }

        private static string ConvertJson<T>(string json)
        {
            if (typeof(T) != typeof(ApiResult))
            {
                var result = json.FromJson<ApiResult>();
                if (result.Status == 0)
                {
                    json = result.Data == null
                         ? string.Empty
                         : result.Data.ToString();
                }
            }

            return json;
        }

        private static string GetBaseUrl(string baseUrl)
        {
            if (!string.IsNullOrWhiteSpace(baseUrl))
                return baseUrl;

            return Setting.Instance.ApiBaseUrl;
        }
    }
}
