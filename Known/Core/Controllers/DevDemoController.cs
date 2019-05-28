using System.Web.Http;
using Known.Core.Services;
using Known.Web;
using Known.WebApi;

namespace Known.Core.Controllers
{
    public class DevDemoController : ApiControllerBase
    {
        private DevDemoService Service
        {
            get { return Container.Resolve<DevDemoService>(); }
        }

        #region DemoGrid
        [HttpPost]
        public object QueryUsers(CriteriaData data)
        {
            var criteria = data.ToPagingCriteria();
            var result = Service.QueryUsers(criteria);
            return ApiResult.ToPageData(result);
        }
        #endregion
    }
}