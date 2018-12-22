using Known.Web;

namespace Known.Platform.WebApi.Controllers.Platform
{
    public class AppController : WebApiController
    {
        public ApiResult GetApiUrl(string apiId)
        {
            var apiUrl = string.Empty;
            if (apiId == "plt")
                apiUrl = "http://localhost:8089";

            return ApiResult.ToData(apiUrl);
        }
    }
}