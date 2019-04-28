using Known.Core.Services;
using Known.Web;

namespace Known.WebApi.Controllers.Develop
{
    public class DevToolController : ApiControllerBase
    {
        private DevToolService Service
        {
            get { return Container.Resolve<DevToolService>(); }
        }

        #region DevDatabase
        public ApiResult QueryDatas(PagingCriteria criteria)
        {
            //var criteria = GetPagingCriteria();
            var result = Service.QueryDatas(criteria);
            return ApiResult.ToPageData(result);
        }
        #endregion
    }
}