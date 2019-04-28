using Known.Core.Services;
using Known.Web;

namespace Known.WebApi.Controllers.Develop
{
    public class DevDemoController : ApiControllerBase
    {
        private DevDemoService Service
        {
            get { return Container.Resolve<DevDemoService>(); }
        }

        #region DemoGrid
        public ApiResult QueryUsers(PagingCriteria criteria)
        {
            //var criteria = GetPagingCriteria();
            var result = Service.QueryUsers(criteria);
            return ApiResult.ToPageData(result);
        }
        #endregion
    }
}