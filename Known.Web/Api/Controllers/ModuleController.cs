using System.Web.Http;
using Known.Platform.Services;

namespace Known.Web.Api.Controllers
{
    public class ModuleController : BaseApiController
    {
        private ModuleService Service
        {
            get { return LoadService<ModuleService>(); }
        }

        [HttpGet]
        public ApiResult GetModule(string mid)
        {
            var module = Service.GetModule(mid);
            return ApiResult.Success(module);
        }
    }
}