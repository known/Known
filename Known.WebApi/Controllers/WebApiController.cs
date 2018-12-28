using System.Web;
using System.Web.Http;
using Known.Extensions;
using Known.Web;

namespace Known.WebApi.Controllers
{
    [RoutePrefix("api")]
    public class WebApiController : BaseApiController
    {
        [HttpGet, Route("{module}/{method}")]
        public ApiResult Get(string module, string method)
        {
            var queries = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var parameters = queries.ToDictionary();
            var data = ServiceUtils.Execute(UserName, module, method, parameters);
            return ApiResult.ToData(data);
        }

        [HttpPost, Route("{module}/{method}")]
        public ApiResult Post(string module, string method)
        {
            var queries = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var parameters = queries.ToDictionary();
            var data = ServiceUtils.Execute(UserName, module, method, parameters);

            if (method.StartsWith("Query"))
            {
                return ApiResult.ToPageData(data as PagingResult);
            }

            return ApiResult.ToData(data);
        }
    }
}