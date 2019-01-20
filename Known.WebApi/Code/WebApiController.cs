using System.Web;
using System.Web.Http;
using Known.Extensions;
using Known.Platform;
using Known.Web;

namespace Known.WebApi
{
    [RoutePrefix("apm")]
    public class WebApiController : BaseApiController
    {
        [HttpGet, Route("{module}/{method}")]
        public ApiResult Get(string module, string method)
        {
            if (module == "User" && method == "GetModules")
            {
                var menus = Menu.GetUserMenus(PlatformService, UserName);
                var codes = Code.GetCodes(PlatformService);
                return ApiResult.ToData(new { menus, codes });
            }

            if (module == "Module" && method == "GetTreeDatas")
            {
                var menus = Menu.GetTreeMenus(PlatformService);
                return ApiResult.ToData(menus);
            }

            var queries = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var executor = new ServiceExecuter(UserName, module, method)
            {
                Parameters = queries.ToDictionary()
            };
            var data = executor.Execute();
            return ApiResult.ToData(data);
        }

        [HttpPost, Route("{module}/{method}")]
        public ApiResult Post(string module, string method)
        {
            var executor = new ServiceExecuter(UserName, module, method);

            if (method.StartsWith("Query"))
            {
                var json = Request.Content.ReadAsStringAsync().Result;
                executor.Parameter = json.FromJson<PagingCriteria>();
                var result = executor.Execute();
                return ApiResult.ToPageData(result as PagingResult);
            }

            var queries = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            executor.Parameters = queries.ToDictionary();
            var data = executor.Execute();
            return ApiResult.ToData(data);
        }
    }
}