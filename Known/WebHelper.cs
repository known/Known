using System;
using System.Net;
using System.Text;

namespace Known
{
    public sealed class WebHelper
    {
        private WebHelper() { }

        public static string Get(string url, string token = null, string proxyUrl = null)
        {
            using (var client = GetWebClient(token, proxyUrl))
            {
                try
                {
                    return client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    Logger.Error(string.Format("{0}\n{1}", url, ex));
                    return ex.Message;
                }
            }
        }

        public static string Post(string url, object data, string token = null, string proxyUrl = null)
        {
            using (var client = GetWebClient(token, proxyUrl))
            {
                try
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

                    client.Headers.Add(HttpRequestHeader.ContentType, contentType);
                    return client.UploadString(url, postData);
                }
                catch (Exception ex)
                {
                    Logger.Info(string.Format("PostData:{0}\n{1}\n", url, Utils.ToJson(data)));
                    Logger.Error(string.Format("{0}\n{1}", url, ex));
                    return ex.Message;
                }
            }
        }

        private static WebClient GetWebClient(string token = null, string proxyUrl = null)
        {
            var client = new WebClient();
            client.Encoding = Encoding.UTF8;
            if (!string.IsNullOrWhiteSpace(token))
                client.Headers.Add(HttpRequestHeader.Authorization, token);
            if (!string.IsNullOrWhiteSpace(proxyUrl))
                client.Proxy = new WebProxy(proxyUrl, false);

            return client;
        }
    }
}
