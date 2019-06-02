using System.Web.Http;
using Known.Core.Services;
using Known.Web;
using Known.WebApi;

namespace Known.Core.Controllers
{
    /// <summary>
    /// 开发示例控制器类。
    /// </summary>
    public class DevDemoController : ApiControllerBase
    {
        private DevDemoService Service
        {
            get { return Container.Resolve<DevDemoService>(); }
        }

        #region DemoGrid
        /// <summary>
        /// 查询示例用户分页数据对象。
        /// </summary>
        /// <param name="data">查询条件对象。</param>
        /// <returns>分页数据对象。</returns>
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