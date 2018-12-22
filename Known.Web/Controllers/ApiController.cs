using System.Collections.Generic;
using System.Web.Mvc;
using Known.Web.Extensions;
using Newtonsoft.Json;

namespace Known.Web.Controllers
{
    [RoutePrefix("api")]
    public class ApiController : AuthorizeController
    {
        [HttpPost, Route("{apiId}/{module}/{method}")]
        public ActionResult Query(string apiId, string module, string method, string query, string isLoad)
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

            var api = GetApiClient(apiId);
            var result = api.Post<ApiResult>("/api/{module}/{method}", criteria);
            if (result.Status == 1)
                return ErrorResult(result.Message);

            return JsonResult(result.Data);
        }

        [HttpGet, Route("{apiId}/{module}/{method}")]
        public ActionResult Get(string apiId, string module, string method, string param = null)
        {
            var api = GetApiClient(apiId);
            var result = api.Get<ApiResult>($"/api/{module}/{method}", FromJson(param));
            if (result.Status == 1)
                return ErrorResult(result.Message);

            return JsonResult(result.Data);
        }

        [HttpPost, Route("{apiId}/{module}/{method}")]
        public ActionResult Post(string apiId, string module, string method, string param = null)
        {
            var api = GetApiClient(apiId);
            var result = api.Post<ApiResult>("/api/{module}/{method}", FromJson(param));
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