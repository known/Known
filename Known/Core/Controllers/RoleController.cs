using System.Web.Http;
using Known.Core.Services;
using Known.Extensions;
using Known.Web;
using Known.WebApi;

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
        /// 查询角色分页数据对象。
        /// </summary>
        /// <param name="data">查询条件对象。</param>
        /// <returns>分页数据对象。</returns>
        [HttpPost]
        public object QueryRoles(CriteriaData data)
        {
            var criteria = data.ToPagingCriteria();
            var result = Service.QueryRoles(criteria);
            return ApiResult.ToPageData(result);
        }

        /// <summary>
        /// 删除一个或多个角色数据。
        /// </summary>
        /// <param name="data">角色对象ID数组。</param>
        /// <returns>删除结果。</returns>
        [HttpPost]
        public object DeleteRoles([FromBody]string data)
        {
            var ids = data.FromJson<string[]>();
            var result = Service.DeleteRoles(ids);
            return ApiResult.Result(result);
        }
        #endregion

        #region Form
        /// <summary>
        /// 获取角色数据对象。
        /// </summary>
        /// <param name="id">角色 id。</param>
        /// <returns>数据对象。</returns>
        public object GetRole(string id)
        {
            return Service.GetRole(id);
        }

        /// <summary>
        /// 保存角色数据。
        /// </summary>
        /// <param name="data">角色数据 JSON。</param>
        /// <returns>保存结果。</returns>
        [HttpPost]
        public object SaveRole([FromBody]string data)
        {
            var model = data.FromJson<dynamic>();
            var result = Service.SaveRole(model);
            return ApiResult.Result(result);
        }
        #endregion
    }
}