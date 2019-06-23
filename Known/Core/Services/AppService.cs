using System;
using System.Collections.Generic;

namespace Known.Core
{
    class AppService : ServiceBase
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

        public List<Application> GetAppList()
        {
            return Database.QueryList<Application>();
        }

        public Result DeleteApp(string id)
        {
            return Result.Success("");
        }
    }
}
