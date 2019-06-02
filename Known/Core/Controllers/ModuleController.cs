using System.Web.Http;
using Known.Core.Services;
using Known.Extensions;
using Known.Web;
using Known.WebApi;

namespace Known.Core.Controllers
{
    /// <summary>
    /// 模块管理控制器类。
    /// </summary>
    public class ModuleController : ApiControllerBase
    {
        private ModuleService Service
        {
            get { return LoadService<ModuleService>(); }
        }

        #region View
        /// <summary>
        /// 获取模块树对象。
        /// </summary>
        /// <returns>模块树对象。</returns>
        [HttpPost]
        public object GetTreeDatas()
        {
            return Menu.GetTreeMenus(PlatformService);
        }

        /// <summary>
        /// 查询模块分页数据对象。
        /// </summary>
        /// <param name="data">查询条件对象。</param>
        /// <returns>分页数据对象。</returns>
        [HttpPost]
        public object QueryModules(CriteriaData data)
        {
            var criteria = data.ToPagingCriteria();
            var result = Service.QueryModules(criteria);
            return ApiResult.ToPageData(result);
        }

        /// <summary>
        /// 删除一个或多个模块数据。
        /// </summary>
        /// <param name="data">模块对象ID数组。</param>
        /// <returns>删除结果。</returns>
        [HttpPost]
        public object DeleteModules([FromBody]string data)
        {
            var ids = data.FromJson<string[]>();
            var result = Service.DeleteModules(ids);
            return ApiResult.Result(result);
        }

        /// <summary>
        /// 上移或下移模块数据。
        /// </summary>
        /// <param name="data">移动模块的id和方向，up-上移、down-下移。</param>
        /// <returns>移动结果。</returns>
        [HttpPost]
        public object MoveModule([FromBody]string data)
        {
            var model = data.FromJson<dynamic>();
            var result = Service.MoveModule((string)model.id, (string)model.direct);
            return ApiResult.Result(result);
        }
        #endregion

        #region Form
        /// <summary>
        /// 获取模块数据对象。
        /// </summary>
        /// <param name="id">模块 id。</param>
        /// <returns>数据对象。</returns>
        public object GetModule(string id)
        {
            return Service.GetModule(id);
        }

        /// <summary>
        /// 保存模块数据。
        /// </summary>
        /// <param name="data">模块数据 JSON。</param>
        /// <returns>保存结果。</returns>
        [HttpPost]
        public object SaveModule([FromBody]string data)
        {
            var model = data.FromJson<dynamic>();
            var result = Service.SaveModule(model);
            return ApiResult.Result(result);
        }
        #endregion
    }
}