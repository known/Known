using System;
using Known;
using Known.Web;

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

            var url = App.ApiUrl + path;
            WebHelper.Post(url, data, null, App.ProxyUrl);
        }
    }
}
