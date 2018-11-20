using System.Web.Http;
using Known.Platform.Business;

namespace Known.Web.Api.Controllers
{
    public class ModuleController : BaseApiController
    {
        private ModuleBusiness Business
        {
            get { return LoadBusiness<ModuleBusiness>(); }
        }

        [HttpGet]
        public ApiResult GetModule(string mid)
        {
            var module = Business.GetModule(mid);
            return ApiResult.Success(module);
        }
    }
}