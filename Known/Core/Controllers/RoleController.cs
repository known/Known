using System.Web.Http;
using Known.Core.Services;
using Known.Web;

namespace Known.Core.Controllers
{
    /// <summary>
    /// 角色管理控制器类。
    /// </summary>
    public class RoleController : ApiControllerBase
    {
        private RoleService Service
        {
            get { return LoadService<RoleService>(); }
        }

        #region View
        /// <summary>
        /// 查询分页数据对象。
        /// </summary>
        /// <param name="data">查询条件对象。</param>
        /// <returns>分页数据对象。</returns>
        [HttpPost]
        public object QueryRoles(CriteriaData data)
        {
            return QueryPagingData(data, c => Service.QueryRoles(c));
        }

        /// <summary>
        /// 删除一个或多个实体对象。
        /// </summary>
        /// <param name="data">实体对象 Id 数组。</param>
        /// <returns>删除结果。</returns>
        [HttpPost]
        public object DeleteRoles([FromBody]string data)
        {
            return PostAction<string[]>(data, d => Service.DeleteRoles(d));
        }
        #endregion

        #region Form
        /// <summary>
        /// 获取实体对象。
        /// </summary>
        /// <param name="id">实体 id。</param>
        /// <returns>实体对象。</returns>
        public object GetRole(string id)
        {
            return Service.GetRole(id);
        }

        /// <summary>
        /// 保存实体对象。
        /// </summary>
        /// <param name="data">实体对象 JSON。</param>
        /// <returns>保存结果。</returns>
        [HttpPost]
        public object SaveRole([FromBody]string data)
        {
            return PostAction<dynamic>(data, d => Service.SaveRole(d));
        }
        #endregion
    }
}