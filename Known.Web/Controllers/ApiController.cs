using System.Web.Mvc;
using Known.Extensions;
using Newtonsoft.Json;

namespace Known.Web.Controllers
{
    public class ApiController : AuthorizeController
    {
        [HttpGet]
        public ActionResult Get(string url, string param = null)
        {
            log.Info($"get: {url} param: {param}");
            var result = Api.Get<ApiResult>(url, param.FromJson<dynamic>());
            if (result.Status == 1)
            {
                log.Info($"error: {result.Message}");
                return ErrorResult(result.Message);
            }

            log.Info($"success: {ToJson(result.Data)}");
            return SuccessResult("", result.Data);
        }

        [HttpPost]
        public ActionResult Post(string url, string param = null)
        {
            log.Info($"post: {url} param: {param}");
            var result = Api.Post<ApiResult>(url, param.FromJson<dynamic>());
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