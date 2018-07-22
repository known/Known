using System.Web.Mvc;
using Known.Web.Extensions;
using Newtonsoft.Json;

namespace Known.Web.Controllers
{
    public class ApiController : AuthorizeController
    {
        [HttpPost]
        public ActionResult Query(string url, string query, string isLoad)
        {
            var param = FromJson(query);
            param.IsLoad = isLoad == "1";
            param.PageIndex = Request.Get<int>("pageIndex");
            param.PageSize = Request.Get<int>("pageSize");
            param.SortField = Request.Get<string>("sortField");
            param.SortOrder = Request.Get<string>("sortOrder");

            var result = Api.Post<ApiResult>(url, param);
            if (result.Status == 1)
                return ErrorResult(result.Message);

            return JsonResult(result.Data);
        }

        [HttpGet]
        public ActionResult Get(string url, string param = null)
        {
            var result = Api.Get<ApiResult>(url, FromJson(param));
            if (result.Status == 1)
                return ErrorResult(result.Message);

            return SuccessResult("", result.Data);
        }

        [HttpPost]
        public ActionResult Post(string url, string param = null)
        {
            var result = Api.Post<ApiResult>(url, FromJson(param));
            if (result.Status == 1)
                return ErrorResult(result.Message);

            return SuccessResult("", result.Data);
        }

        private dynamic FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            return JsonConvert.DeserializeObject<dynamic>(json);
        }

        private string ToJson(dynamic data)
        {
            if (data == null)
                return string.Empty;

            return JsonConvert.SerializeObject(data);
        }
    }
}