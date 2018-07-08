using System.Web.Mvc;
using Known.Web.Filters;
using Newtonsoft.Json;

namespace Known.Web.Controllers
{
    public class ApiController : BaseController
    {
        [HttpGet]
        [LoginAuthorize]
        public ActionResult Get(string url, dynamic param = null)
        {
            log.Info($"get: {url} param: {ToJson(param)}");
            var result = Api.Get<ApiResult>(url, param);
            if (result.Status == 1)
            {
                log.Info($"error: {result.Message}");
                return ErrorResult(result.Message);
            }

            log.Info($"success: {ToJson(result.Data)}");
            return SuccessResult("", result.Data);
        }

        [HttpPost]
        [LoginAuthorize]
        public ActionResult Post(string url, dynamic param = null)
        {
            log.Info($"post: {url} param: {ToJson(param)}");
            var result = Api.Post<ApiResult>(url, param);
            if (result.Status == 1)
            {
                log.Info($"error: {result.Message}");
                return ErrorResult(result.Message);
            }

            log.Info($"success: {ToJson(result.Data)}");
            return SuccessResult("", result.Data);
        }

        private string ToJson(dynamic data)
        {
            if (data == null)
                return string.Empty;

            return JsonConvert.SerializeObject(data);
        }
    }
}