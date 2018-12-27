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
            var querys = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var parameters = querys.ToDictionary();
            var data = ServiceUtils.Execute(UserName, module, method, parameters);
            return ApiResult.ToData(data);
        }

        [HttpPost, Route("{module}/{method}")]
        public ApiResult Post(string module, string method)
        {
            var querys = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var parameters = querys.ToDictionary();
            var data = ServiceUtils.Execute(UserName, module, method, parameters);
            return ApiResult.ToData(data);
        }
    }
}