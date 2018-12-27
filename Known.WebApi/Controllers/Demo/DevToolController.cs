using Known.Platform.Services;
using Known.Web;

namespace Known.Platform.WebApi.Controllers.Demo
{
    public class DevToolController : WebApiController
    {
        private DevToolService Service
        {
            get { return LoadService<DevToolService>(); }
        }

        public ApiResult QueryDatas(PagingCriteria criteria)
        {
            var result = Service.QueryDatas(criteria);
            return ApiResult.ToPageData(result);
        }
    }
}