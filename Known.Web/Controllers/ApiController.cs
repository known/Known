using System.Collections.Generic;
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
            var sortField = Request.Get<string>("sortField");
            var sortOrder = Request.Get<string>("sortOrder");
            var sorts = new List<string>();
            if (!string.IsNullOrWhiteSpace(sortField))
            {
                sorts.Add($"{sortField} {sortOrder}");
            }

            var criteria = new PagingCriteria
            {
                IsLoad = isLoad == "1",
                PageIndex = Request.Get<int>("pageIndex"),
                PageSize = Request.Get<int>("pageSize"),
                OrderBys = sorts.ToArray(),
                Parameters = FromJson(query)
            };

            var result = Api.Post<ApiResult>(url, criteria);
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

            return JsonResult(result.Data);
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