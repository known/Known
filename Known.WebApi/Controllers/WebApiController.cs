using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using Known.Extensions;
using Known.Platform;
using Known.Platform.Services;
using Known.Web;
using Known.WebApi.Models;
using Newtonsoft.Json;

namespace Known.WebApi.Controllers
{
    [RoutePrefix("api")]
    public class WebApiController : BaseApiController
    {
        [HttpGet, Route("{module}/{method}")]
        public ApiResult Get(string module, string method)
        {
            if (module == "User" && method == "GetModules")
                return GetUserModules();

            if (module == "Module" && method == "GetTreeDatas")
                return GetTreeDatas();

            var queries = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var parameters = queries.ToDictionary();
            var data = ServiceUtils.Execute(UserName, module, method, parameters);
            return ApiResult.ToData(data);
        }

        [HttpPost, Route("{module}/{method}")]
        public ApiResult Post(string module, string method)
        {
            var json = Request.Content.ReadAsStringAsync().Result;

            if (method.StartsWith("Query"))
            {
                var parameter = json.FromJson<PagingCriteria>();
                var result = ServiceUtils.Execute(UserName, module, method, parameter);
                return ApiResult.ToPageData(result as PagingResult);
            }

            var queries = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var parameters = queries.ToDictionary();
            var data = ServiceUtils.Execute(UserName, module, method, parameters);
            return ApiResult.ToData(data);
        }

        private static dynamic FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            return JsonConvert.DeserializeObject<dynamic>(json);
        }

        private ApiResult GetUserModules()
        {
            var menus = new List<Menu>();
            var modules = LoadService<UserService>().GetModules();
            if (modules != null && modules.Count > 0)
            {
                var index = 0;
                foreach (var item in modules)
                {
                    var menu = Menu.GetMenu(item);
                    menu.expanded = index == 0;
                    menus.Add(menu);
                    Menu.SetSubModules(menus, item, menu);
                    index++;
                }
            }

            var codes = new Dictionary<string, object>();
            codes.Add("ViewType", Code.GetEnumCodes<ViewType>());

            return ApiResult.ToData(new { menus, codes });
        }

        private ApiResult GetTreeDatas()
        {
            var menus = new List<Menu>();
            var modules = LoadService<ModuleService>().GetModules(true);
            if (modules != null && modules.Count > 0)
            {
                foreach (var item in modules)
                {
                    var menu = Menu.GetMenu(item);
                    menu.expanded = item.ParentId == "-1" || item.ParentId == "0";
                    menus.Add(menu);
                }
            }

            return ApiResult.ToData(menus);
        }
    }
}