using Known.WebApi;

namespace Known.Core.Controllers
{
    /// <summary>
    /// 开发工具管理控制器类。
    /// </summary>
    public class DevToolController : ApiControllerBase
    {
        private DevToolService Service
        {
            get { return Container.Resolve<DevToolService>(); }
        }

        #region DevDatabase
        /// <summary>
        /// 查询分页数据对象。
        /// </summary>
        /// <param name="criteria">查询条件对象。</param>
        /// <returns>分页查询结果。</returns>
        protected override PagingResult QueryDatas(PagingCriteria criteria)
        {
            return Service.QueryDatas(criteria);
        }
        #endregion
    }
}