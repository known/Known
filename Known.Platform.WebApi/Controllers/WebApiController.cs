using System.Collections.Generic;
using System.Web.Http;
using Known.Web;
using Known.WebApi;

namespace Known.Platform.WebApi.Controllers
{
    [RoutePrefix("api")]
    public class WebApiController : BaseApiController
    {
        [HttpGet, Route("{module}/{method}")]
        public ApiResult Get(string module, string method)
        {
            var parameters = new List<object>();
            foreach (var key in Request.Properties)
            {
                //parameters.Add(request.QueryString[key]);
            }
            var data = ServiceUtils.Execute(UserName, module, method, parameters.ToArray());
            return ApiResult.ToData(data);
        }

        [HttpPost, Route("{module}/{method}")]
        public ApiResult Post(string module, string method)
        {
            var parameters = new List<object>();
            foreach (var key in Request.Properties)
            {
                //parameters.Add(request.QueryString[key]);
            }
            var data = ServiceUtils.Execute(UserName, module, method, parameters.ToArray());
            return ApiResult.ToData(data);
        }
    }
}