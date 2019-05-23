using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.Mvc;
using Known.Extensions;
using Known.Platform;
using Known.Web;
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
                var menus = Menu.GetUserMenus(PlatformService, UserName);
                var codes = Code.GetCodes(PlatformService);
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
                var menus = Menu.GetTreeMenus(PlatformService);
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
                var executor = new ServiceExecuter(UserName, module, method);
                var data = executor.Execute(param);
                return JsonResult(data);
            }

            var result = api.Get<ApiResult>($"/api/{module}/{method}", param);
            if (result.Status == 1)
                return ErrorResult(result.Message);

            return JsonResult(result.Data);
        }

        private ActionResult Post(ApiClient api, string module, string method)
        {
            if (Setting.Instance.IsMonomer)
            {
                var executor = new ServiceExecuter(UserName, module, method);
                object data;
                if (method.StartsWith("Save"))
                    data = executor.Execute(GetForm(Request.Form));
                else
                    data = executor.Execute(GetParam(Request.Form));

                return JsonResult(data);
            }

            var param = method.StartsWith("Save")
                      ? GetForm(Request.Form)
                      : GetParam(Request.Form);
            var result = api.Post<ApiResult>($"/api/{module}/{method}", param);
            if (result.Status == 1)
                return ErrorResult(result.Message);

            return SuccessResult(result.Message, result.Data);
        }

        private ActionResult Query(ApiClient api, string module, string method)
        {
            var criteria = GetPagingCriteria();

            if (Setting.Instance.IsMonomer)
            {
                var executor = new ServiceExecuter(UserName, module, method);
                var data = executor.Execute(criteria);
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

        private static dynamic GetForm(NameValueCollection collection)
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