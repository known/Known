using System.Collections.Generic;
using System.Web.Mvc;
using Known.Web.Extensions;
using Newtonsoft.Json;

namespace Known.Web.Controllers
{
    [RoutePrefix("api")]
    public class ApiController : AuthorizeController
    {
        [HttpPost, Route("{module}/{method}")]
        public ActionResult Query(string route, string query, string isLoad)
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
                Parameter = FromJson(query)
            };

            var result = Api.Post<ApiResult>("/api/" + route, criteria);
            if (result.Status == 1)
                return ErrorResult(result.Message);

            return JsonResult(result.Data);
        }

        [HttpGet, Route("{module}/{method}")]
        public ActionResult Get(string module, string method, string param = null)
        {
            var result = Api.Get<ApiResult>($"/api/{module}/{method}", FromJson(param));
            if (result.Status == 1)
                return ErrorResult(result.Message);

            return JsonResult(result.Data);
        }

        [HttpPost, Route("{module}/{method}")]
        public ActionResult Post(string module, string method, string param = null)
        {
            var result = Api.Post<ApiResult>("/api/{module}/{method}", FromJson(param));
            if (result.Status == 1)
                return ErrorResult(result.Message);

            return SuccessResult(result.Message, result.Data);
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