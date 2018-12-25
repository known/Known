using System.Collections.Generic;
using Known.Platform;
using Known.Platform.Services;

namespace Known.Web
{
    public class PltApiHelper
    {
        private static readonly Dictionary<string, string> apiBaseUrls = new Dictionary<string, string>();
        private readonly Context context;
        private readonly ApiClient api;

        public PltApiHelper(Context context, ApiClient api)
        {
            this.context = context;
            this.api = api;
        }

        public User GetUser(string userName)
        {
            if (Setting.Instance.IsMonomer)
            {
                var service = ObjectFactory.CreateService<UserService>(context);
                return service.GetUser(userName);
            }

            return api.Get<User>("/api/User/GetUser", new { userName });
        }

        public string GetApiBaseUrl(string apiId)
        {
            if (string.IsNullOrWhiteSpace(apiId))
                return null;

            if (Setting.Instance.IsMonomer)
            {
                var service = ObjectFactory.CreateService<AppService>(context);
                return service.GetApiUrl(apiId);
            }

            if (!apiBaseUrls.ContainsKey(apiId))
            {
                apiBaseUrls[apiId] = api.Get<string>("/api/App/GetApiUrl", new { apiId });
            }

            return apiBaseUrls[apiId];
        }

        public ApiResult SignIn(string userName, string password)
        {
            if (Setting.Instance.IsMonomer)
            {
                var service = ObjectFactory.CreateService<UserService>(context);
                var result = service.SignIn(userName, password);
                if (!result.IsValid)
                    return ApiResult.Error(result.Message);

                return ApiResult.ToData(result.Data);
            }

            return api.Get<ApiResult>("/api/User/SignIn", new { userName, password });
        }

        public Module GetModule(string mid)
        {
            if (Setting.Instance.IsMonomer)
            {
                var service = ObjectFactory.CreateService<ModuleService>(context);
                return service.GetModule(mid);
            }

            return api.Get<Module>("/api/Module/GetModule", new { mid });
        }
    }
}