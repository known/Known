using System.Collections.Generic;
using Known.Web;

namespace Known.Platform.Helpers
{
    class ApiPlatformHelper : PlatformHelper
    {
        private static readonly Dictionary<string, string> apiBaseUrls = new Dictionary<string, string>();
        private readonly ApiClient client;

        public ApiPlatformHelper(ApiClient client)
        {
            if (client == null)
            {
                client = new ApiClient();
            }

            this.client = client;
        }

        public override string GetApiBaseUrl(string apiId)
        {
            if (string.IsNullOrWhiteSpace(apiId))
                return null;

            if (!apiBaseUrls.ContainsKey(apiId))
            {
                apiBaseUrls[apiId] = client.Get<string>("/api/App/GetApiUrl", new { apiId });
            }

            return apiBaseUrls[apiId];
        }

        public override Dictionary<string, object> GetCodes()
        {
            return client.Get<Dictionary<string, object>>("/api/App/GetCodes");
        }

        public override Module GetModule(string id)
        {
            return client.Get<Module>("/api/Module/GetModule", new { id });
        }

        public override List<Module> GetModules()
        {
            return client.Get<List<Module>>("/api/Module/GetModules");
        }

        public override List<Module> GetUserModules(string userName)
        {
            return client.Get<List<Module>>("/api/User/GetUserModules", new { userName });
        }

        public override User GetUser(string userName)
        {
            return client.Get<User>("/api/User/GetUser", new { userName });
        }

        public override void SaveUser(User user)
        {
            client.Post("/api/User/SaveUser", user);
        }
    }
}
