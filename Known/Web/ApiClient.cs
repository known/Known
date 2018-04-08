using Known.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace Known.Web
{
    /// <summary>
    /// Api客户端。
    /// </summary>
    public class ApiClient
    {
        private readonly int lengthThreshold = 5000;
        private readonly CompressionMethod defaultCompressionMethod = CompressionMethod.GZip;
        private AuthenticationHeaderValue authorization;

        /// <summary>
        /// 构造函数，创建一个Api客户端实例。
        /// </summary>
        public ApiClient() { }

        /// <summary>
        /// 构造函数，创建一个Api客户端实例。
        /// </summary>
        /// <param name="token">访问Api的Token。</param>
        public ApiClient(string token)
        {
            authorization = new AuthenticationHeaderValue("Token", token);
        }

        /// <summary>
        /// 构造函数，创建一个Api客户端实例。
        /// </summary>
        /// <param name="account">访问Api的账号。</param>
        /// <param name="password">访问Api的密码。</param>
        public ApiClient(string account, string password)
        {
            var authParameter = string.Format("{0}:{1}", account, password);
            var bytes = Encoding.Default.GetBytes(authParameter);
            var parameter = Convert.ToBase64String(bytes);
            authorization = new AuthenticationHeaderValue("Basic", parameter);
        }

        /// <summary>
        /// GET请求数据。
        /// </summary>
        /// <param name="url">请求的URL。</param>
        /// <param name="args">请求的参数。</param>
        /// <returns>请求的结果。</returns>
        public string Get(string url, IDictionary<string, object> args = null)
        {
            return Request("GET", url, args);
        }

        /// <summary>
        /// GET请求返回指定类型的数据。
        /// </summary>
        /// <typeparam name="T">返回的数据类型。</typeparam>
        /// <param name="url">请求的URL。</param>
        /// <param name="args">请求的参数。</param>
        /// <returns>指定类型的数据。</returns>
        public T Get<T>(string url, IDictionary<string, object> args = null)
        {
            return Get(url, args).FromJson<T>();
        }

        /// <summary>
        /// POST请求数据。
        /// </summary>
        /// <param name="url">请求的URL。</param>
        /// <param name="data">请求的参数。</param>
        /// <returns>请求的结果。</returns>
        public string Post(string url, object data)
        {
            return Request("POST", url, data);
        }

        /// <summary>
        /// POST请求返回指定类型的数据。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">请求的URL。</param>
        /// <param name="data">请求的参数。</param>
        /// <returns>指定类型的数据。</returns>
        public T Post<T>(string url, object data)
        {
            return Post(url, data).FromJson<T>();
        }

        private string Request(string method, string url, object args, CompressionMethod compressionMethod = CompressionMethod.Automatic)
        {
            url += "?" + GetQueryString(method, args);
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
                    var data = args.ToJson();
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

        private static string GetQueryString(string method, object args)
        {
            var dic = new Dictionary<string, object>();
            if (args != null)
            {
                if (method == "GET")
                    ((IDictionary<string, object>)args).ToList().ForEach(e => dic.Add(e.Key, e.Value));
                else
                    dic.Add("body", args.ToJson());
            }

            dic.Add("timestamp", DateTime.Now.ToTimestamp());
            dic.Add("nonce", Utils.NewGuid);
            dic.Add("sign", dic.ToMd5Signature());

            var values = dic.Where(d => d.Key != "body")
                            .Select(d => string.Format("{0}={1}", d.Key, HttpUtility.UrlEncode(d.Value.ToString())));
            return string.Join("&", values);
        }
    }
}
