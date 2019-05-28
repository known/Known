using Known.Core.Services;
using Known.Web;
using Known.WebApi;

namespace Known.Core.Controllers
{
    public class DevToolController : ApiControllerBase
    {
        private DevToolService Service
        {
            get { return Container.Resolve<DevToolService>(); }
        }

        #region DevDatabase
        public object QueryDatas(CriteriaData data)
        {
            var criteria = data.ToPagingCriteria();
            var result = Service.QueryDatas(criteria);
            return ApiResult.ToPageData(result);
        }
        #endregion
    }
}