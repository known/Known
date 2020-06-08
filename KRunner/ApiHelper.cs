using System;
using System.Net;
using System.Text;
using Known;

namespace KRunner
{
    class ApiHelper
    {
        public static AppInfo App { get; set; }

        public static void PostStatus(IJob job, string status)
        {
            Post("/log/status", new
            {
                ClientId = App.Name,
                ClientName = App.Description,
                Job = job.Config,
                Status = status
            });
        }

        public static void PostError(IJob job, Exception ex)
        {
            Post("/log/error", new
            {
                ClientId = App.Name,
                ClientName = App.Description,
                Job = job.Config,
                Exception = ex.ToString()
            });
        }

        private static void Post(string path, object data)
        {
            if (string.IsNullOrWhiteSpace(App.ApiUrl))
                return;

            var address = App.ApiUrl + path;
            using (var client = new WebClient())
            {
                try
                {
                    client.Encoding = Encoding.UTF8;
                    //if (!string.IsNullOrWhiteSpace(token))
                    //    client.Headers.Add(HttpRequestHeader.Authorization, token);
                    if (!string.IsNullOrWhiteSpace(App.ProxyUrl))
                        client.Proxy = new WebProxy(App.ProxyUrl, false);

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
                    client.UploadString(address, postData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}\n{1}\n{2}", address, Utils.ToJson(data), ex);
                }
            }
        }
    }
}
