using Known.Platform.Services;
using Known.Web;

namespace Known.Platform.WebApi.Controllers.Platform
{
    public class AppController : WebApiController
    {
        private AppService Service
        {
            get { return LoadService<AppService>(); }
        }

        public ApiResult GetApiUrl(string apiId)
        {
            var apiUrl = Service.GetApiUrl(apiId);
            return ApiResult.ToData(apiUrl);
        }
    }
}