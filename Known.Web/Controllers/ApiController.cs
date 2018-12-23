using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.Mvc;
using Known.Web.Extensions;
using Newtonsoft.Json;

namespace Known.Web.Controllers
{
    [RoutePrefix("api")]
    public class ApiController : AuthorizeController
    {
        [HttpGet, Route("{apiId}/{module}/{method}")]
        public ActionResult Get(string apiId, string module, string method)
        {
            var api = GetApiClient(apiId);
            return Get(api, module, method);
        }

        [HttpPost, Route("{apiId}/{module}/{method}")]
        public ActionResult Post(string apiId, string module, string method)
        {
            var api = GetApiClient(apiId);
            if (method.StartsWith("Query"))
                return Query(api, module, method);

            if (method.StartsWith("Get"))
                return Get(api, module, method);

            var param = GetParam(Request.Form);
            var result = api.Post<ApiResult>($"/api/{module}/{method}", param);
            if (result.Status == 1)
                return ErrorResult(result.Message);

            return SuccessResult(result.Message, result.Data);
        }

        private ActionResult Get(ApiClient api, string module, string method)
        {
            var param = GetParam(Request.QueryString);
            var result = api.Get<ApiResult>($"/api/{module}/{method}", param);
            if (result.Status == 1)
                return ErrorResult(result.Message);

            return JsonResult(result.Data);
        }

        private ActionResult Query(ApiClient api, string module, string method)
        {
            var query = Request.Get<string>("query");
            var isLoad = Request.Get<string>("isLoad");
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

            var result = api.Post<ApiResult>($"/api/{module}/{method}", criteria);
            if (result.Status == 1)
                return ErrorResult(result.Message);

            return JsonResult(result.Data);
        }

        private static dynamic FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            return JsonConvert.DeserializeObject<dynamic>(json);
        }

        private static dynamic GetParam(NameValueCollection collection)
        {
            if (collection == null)
                return null;

            var dic = new Dictionary<string, object>();
            foreach (var item in collection.AllKeys)
            {
                if (item == "_")
                    continue;

                dic[item] = collection[item];
            }

            if (dic.Count == 0)
                return null;

            return dic;
        }

        private static string ToJson(dynamic data)
        {
            if (data == null)
                return string.Empty;

            return JsonConvert.SerializeObject(data);
        }
    }
}