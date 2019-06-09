using System.Web.Http;
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
        /// 查询分页数据对象。
        /// </summary>
        /// <param name="criteria">查询条件对象。</param>
        /// <returns>分页查询结果。</returns>
        protected override PagingResult QueryDatas(PagingCriteria criteria)
        {
            return Service.QueryModules(criteria);
        }

        /// <summary>
        /// 删除一个或多个实体对象。
        /// </summary>
        /// <param name="ids">实体对象 Id 数组。</param>
        /// <returns>删除结果。</returns>
        protected override Result DeleteDatas(string[] ids)
        {
            return Service.DeleteModules(ids);
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
        /// 获取实体对象。
        /// </summary>
        /// <param name="id">实体 id。</param>
        /// <returns>实体对象。</returns>
        public override object GetData(string id)
        {
            return Service.GetModule(id);
        }

        /// <summary>
        /// 保存实体对象。
        /// </summary>
        /// <param name="model">实体对象。</param>
        /// <returns>保存结果。</returns>
        protected override Result SaveData(dynamic model)
        {
            return Service.SaveModule(model);
        }
        #endregion
    }
}