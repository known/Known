using System;
using System.Net;
using System.Text;

namespace Known.Web
{
    public class WebHelper
    {
        public static void Post(string url, object data, string token = null, string proxyUrl = null)
        {
            using (var client = new WebClient())
            {
                try
                {
                    client.Encoding = Encoding.UTF8;
                    if (!string.IsNullOrWhiteSpace(token))
                        client.Headers.Add(HttpRequestHeader.Authorization, token);
                    if (!string.IsNullOrWhiteSpace(proxyUrl))
                        client.Proxy = new WebProxy(proxyUrl, false);

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
                    client.UploadString(url, postData);
                }
                catch (Exception ex)
                {
                    Logger.Error(string.Format("{0}\n{1}\n{2}", url, Utils.ToJson(data), ex));
                }
            }
        }
    }
}
