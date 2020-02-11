using System;
using System.Collections.Generic;
using Known.Web.Entities;

namespace Known.Web.Services
{
    public class AppService : ServiceBase
    {
        public AppService(Context context) : base(context)
        {
        }

        public string GetApiUrl(string apiId)
        {
            var apiUrl = string.Empty;
            if (apiId == "plt")
                apiUrl = "http://localhost:8089";

            return apiUrl;
        }

        public List<TApplication> GetAppList()
        {
            return Database.QueryList<TApplication>();
        }

        public Result DeleteApp(string id)
        {
            return Result.Success("");
        }
    }
}
