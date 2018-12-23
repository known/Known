using System.Collections.Generic;
using System.Web.Http;
using Known.Web;
using Known.WebApi;

namespace Known.Platform.WebApi.Controllers
{
    [RoutePrefix("api1")]
    public class WebApiController : BaseApiController
    {
        [HttpGet, Route("{module}/{method}")]
        public ApiResult Get(string module, string method)
        {
            var service = GetService(module);
            if (service == null)
                return ApiResult.Error($"暂未实现{module}模块！");

            var func = service.GetType().GetMethod(method);
            if (func == null)
                return ApiResult.Error($"暂未实现{module}模块！");

            var parameters = new List<object>();
            foreach (var key in Request.Properties)
            {
                //parameters.Add(request.QueryString[key]);
            }
            var data = func.Invoke(service, parameters.ToArray());
            return ApiResult.ToData(data);
        }

        [HttpPost, Route("{module}/{method}")]
        public ApiResult Post(string module, string method)
        {
            var service = GetService(module);
            if (service == null)
                return ApiResult.Error($"暂未实现{module}模块！");

            var func = service.GetType().GetMethod(method);
            if (func == null)
                return ApiResult.Error($"暂未实现{module}模块！");

            var parameters = new List<object>();
            foreach (var key in Request.Properties)
            {
                //parameters.Add(request.QueryString[key]);
            }
            var data = func.Invoke(service, parameters.ToArray());
            return ApiResult.ToData(data);
        }

        protected object GetService(string module)
        {
            var service = Container.Load($"{module}Service");
            if (service != null)
            {
                ((ServiceBase)service).Context.UserName = UserName;
            }
            return service;
        }
    }
}