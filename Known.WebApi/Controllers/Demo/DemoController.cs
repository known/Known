using Known.Platform.Services;
using Known.Web;

namespace Known.Platform.WebApi.Controllers.Demo
{
    public class DemoController : WebApiController
    {
        private DemoService Service
        {
            get { return LoadService<DemoService>(); }
        }

        public ApiResult QueryUsers(PagingCriteria criteria)
        {
            var result = Service.QueryUsers(criteria);
            return ApiResult.ToPageData(result);
        }
    }
}