using System.Collections.Generic;
using Known.Platform;
using Known.Platform.Services;
using Known.Web;

namespace Known.WebMvc
{
    public class PltApiHelper
    {
        private static readonly Dictionary<string, string> apiBaseUrls = new Dictionary<string, string>();
        private readonly ApiClient client;
        private readonly Context context;

        public PltApiHelper(ApiClient api, string userName)
        {
            if (Setting.Instance.IsMonomer)
            {
                context = Context.Create(userName);
            }
            else
            {
                this.client = api;
            }
        }

        public User GetUser(string userName)
        {
            if (Setting.Instance.IsMonomer)
            {
                var service = LoadService<UserService>();
                return service.GetUser(userName);
            }

            return client.Get<User>("/api/User/GetUser", new { userName });
        }

        public string GetApiBaseUrl(string apiId)
        {
            if (string.IsNullOrWhiteSpace(apiId))
                return null;

            if (Setting.Instance.IsMonomer)
            {
                var service = LoadService<AppService>();
                return service.GetApiUrl(apiId);
            }

            if (!apiBaseUrls.ContainsKey(apiId))
            {
                apiBaseUrls[apiId] = client.Get<string>("/api/App/GetApiUrl", new { apiId });
            }

            return apiBaseUrls[apiId];
        }

        public ApiResult SignIn(string userName, string password)
        {
            if (Setting.Instance.IsMonomer)
            {
                var service = LoadService<UserService>();
                var result = service.SignIn(userName, password);
                if (!result.IsValid)
                    return ApiResult.Error(result.Message);

                return ApiResult.ToData(result.Data);
            }

            return client.Get<ApiResult>("/api/User/SignIn", new { userName, password });
        }

        public Module GetModule(string mid)
        {
            if (Setting.Instance.IsMonomer)
            {
                var service = LoadService<ModuleService>();
                return service.GetModule(mid);
            }

            return client.Get<Module>("/api/Module/GetModule", new { mid });
        }

        private T LoadService<T>() where T : ServiceBase
        {
            return ObjectFactory.CreateService<T>(context);
        }
    }
}