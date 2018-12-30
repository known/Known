using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.Mvc;
using Known.Extensions;
using Known.Platform.Models;
using Known.Platform.Services;
using Known.Web;
using Known.Web.Extensions;
using Newtonsoft.Json;

namespace Known.WebMvc.Controllers
{
    [RoutePrefix("api")]
    public class ApiController : AuthorizeController
    {
        [HttpGet, Route("{apiId}/{module}/{method}")]
        public ActionResult Get(string apiId, string module, string method)
        {
            if (module == "User" && method == "GetModules")
            {
                var service = Container.Resolve<UserService>("UserService");
                var menus = Menu.GetUserMenus(service);
                var codes = Code.GetCodes();
                return JsonResult(new { menus, codes });
            }

            var api = GetApiClient(apiId);
            return Get(api, module, method);
        }

        [HttpPost, Route("{apiId}/{module}/{method}")]
        public ActionResult Post(string apiId, string module, string method)
        {
            if (module == "Module" && method == "GetTreeDatas")
            {
                var service = Container.Resolve<ModuleService>("ModuleService");
                var menus = Menu.GetMenus(service);
                return JsonResult(menus);
            }

            var api = GetApiClient(apiId);
            if (method.StartsWith("Query"))
                return Query(api, module, method);

            if (method.StartsWith("Get"))
                return Get(api, module, method);

            return Post(api, module, method);
        }

        private ActionResult Get(ApiClient api, string module, string method)
        {
            var param = GetParam(Request.QueryString);
            if (Setting.Instance.IsMonomer)
            {
                var data = ServiceUtils.Execute(UserName, module, method, param);
                return JsonResult(data);
            }

            var result = api.Get<ApiResult>($"/api/{module}/{method}", param);
            if (result.Status == 1)
                return ErrorResult(result.Message);

            return JsonResult(result.Data);
        }

        private ActionResult Post(ApiClient api, string module, string method)
        {
            var param = method.StartsWith("Save")
                      ? FromForm(Request.Form)
                      : GetParam(Request.Form);
            if (Setting.Instance.IsMonomer)
            {
                var data = ServiceUtils.Execute(UserName, module, method, param);
                return JsonResult(data);
            }

            var result = api.Post<ApiResult>($"/api/{module}/{method}", param);
            if (result.Status == 1)
                return ErrorResult(result.Message);

            return SuccessResult(result.Message, result.Data);
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

            if (Setting.Instance.IsMonomer)
            {
                var parameters = new Dictionary<string, object>();
                parameters.Add("criteria", criteria);
                var data = ServiceUtils.Execute(UserName, module, method, parameters);
                return PageResult(data as PagingResult);
            }

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

        private static dynamic FromForm(NameValueCollection collection)
        {
            var param = GetParam(collection);
            var json = param.ToJson();
            return FromJson(json);
        }

        private static Dictionary<string, object> GetParam(NameValueCollection collection)
        {
            var dic = collection.ToDictionary();
            if (dic == null)
                return null;

            if (dic.ContainsKey("_"))
                dic.Remove("_");

            if (dic.Count == 0)
                return null;

            return dic;
        }
    }
}