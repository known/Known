/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * ------------------------------------------------------------------------------- */

using System.Collections.Generic;
#if NET35
using System.Net;
#else
using System.Net.Http;
#endif
using System.Text;

namespace Known
{
    public class ApiFile
    {
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public byte[] Bytes { get; set; }
    }

    public class ApiHelper
    {
        private readonly string baseUrl;
        private readonly string token;

        public ApiHelper(string baseUrl)
        {
            this.baseUrl = baseUrl;
            this.token = Config.MacAddress;
        }

        public ApiHelper(string baseUrl, string token)
        {
            this.baseUrl = baseUrl;
            this.token = token;
        }

        public string Get(string path)
        {
            return HttpHelper.Get(baseUrl + path, token);
        }

        public T Get<T>(string path)
        {
            var json = Get(path);
            return Utils.FromJson<T>(json);
        }

        public string Post(string path, object data = null)
        {
            return HttpHelper.Post(baseUrl + path, data, token);
        }

        public Result Post(string path, object data = null, List<ApiFile> files = null)
        {
            var json = PostAction(path, data, files);
            return Utils.FromJson<Result>(json);
        }

        private string PostAction(string path, object data = null, List<ApiFile> files = null)
        {
            return Post(path, new
            {
                data = data.GetType() == typeof(string) ? data : Utils.ToJson(data),
                files = Utils.ToJson(files)
            });
        }
    }

    public sealed class HttpHelper
    {
        private HttpHelper() { }

        public static string Get(string url, string token = null, string proxyUrl = null, Encoding encoding = null)
        {
            using (var client = GetWebClient(token, proxyUrl))
            {
#if NET35
                if (encoding != null)
                    client.Encoding = encoding;
                return client.DownloadString(url);
#else
                if (encoding == null)
                    return client.GetStringAsync(url).Result;

                var bytes = client.GetByteArrayAsync(url).Result;
                return encoding.GetString(bytes);
#endif
            }
        }

        public static string Post(string url, object data = null, string token = null, string proxyUrl = null, Encoding encoding = null)
        {
            if (data == null)
            {
                using (var client = GetWebClient(token, proxyUrl))
                {
#if NET35
                    if (encoding != null)
                        client.Encoding = encoding;
                    return client.UploadString(url, null);
#else
                    var result = client.PostAsync(url, null).Result.Content;
                    if (encoding == null)
                        return result.ReadAsStringAsync().Result;

                    var bytes = result.ReadAsByteArrayAsync().Result;
                    return encoding.GetString(bytes);
#endif
                }
            }
            else
            {
                var contentType = string.Empty;
                var postData = string.Empty;
                if (data.GetType() == typeof(string))
                {
                    contentType = "application/x-www-form-urlencoded";
                    postData = data.ToString();
                }
                else
                {
                    contentType = "application/json";
                    postData = Utils.ToJson(data);
                }

                using (var client = GetWebClient(token, proxyUrl))
                {
#if NET35
                    if (encoding != null)
                        client.Encoding = encoding;
                    client.Headers.Add(HttpRequestHeader.ContentType, contentType);
                    return client.UploadString(url, postData);
#else
                
                    using (var content = new StringContent(postData, Encoding.UTF8, contentType))
                    {
                        var result = client.PostAsync(url, content).Result.Content;
                        if (encoding == null)
                            return result.ReadAsStringAsync().Result;

                        var bytes = result.ReadAsByteArrayAsync().Result;
                        return encoding.GetString(bytes);
                    }
#endif
                }
            }
        }

#if NET35
        private static WebClient GetWebClient(string token = null, string proxyUrl = null)
        {
            var client = new WebClient();
            client.Encoding = Encoding.UTF8;
            if (!string.IsNullOrEmpty(token))
                client.Headers.Add(Constants.KeyToken, token);
            if (!string.IsNullOrEmpty(proxyUrl))
                client.Proxy = new WebProxy(proxyUrl, false);

            return client;
        }
#else
        private static HttpClient GetWebClient(string token = null, string proxyUrl = null)
        {
            var client = new HttpClient();
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Add(Constants.KeyToken, token);

            return client;
        }
#endif
    }
}