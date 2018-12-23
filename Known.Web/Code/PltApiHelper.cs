using System.Collections.Generic;
using Known.Platform;

namespace Known.Web
{
    public class PltApiHelper
    {
        private static readonly Dictionary<string, string> apiBaseUrls = new Dictionary<string, string>();

        public static User GetUser(string userName)
        {
            var api = new ApiClient();
            return api.Get<User>("/api/User/GetUser", new { userName });
        }

        public static string GetApiBaseUrl(string apiId)
        {
            if (string.IsNullOrWhiteSpace(apiId))
                return null;

            if (!apiBaseUrls.ContainsKey(apiId))
            {
                var api = new ApiClient();
                apiBaseUrls[apiId] = api.Get<string>("/api/App/GetApiUrl", new { apiId });
            }

            return apiBaseUrls[apiId];
        }

        public static ApiResult SignIn(ApiClient api, string userName, string password)
        {
            return api.Get<ApiResult>("/api/User/SignIn", new { userName, password });
        }

        public static Module GetModule(ApiClient api, string mid)
        {
            return api.Get<Module>("/api/Module/GetModule", new { mid });
        }
    }
}