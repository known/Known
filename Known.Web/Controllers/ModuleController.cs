using Known.Core;
using Known.Web.Mvc;
using Known.Web.Services;

namespace Known.Web.Controllers
{
    /// <summary>
    /// 模块管理控制器类。
    /// </summary>
    public class ModuleController : MvcControllerBase
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
        public ActionResult GetTreeDatas()
        {
            return Json(Menu.GetTreeMenus(PlatformService));
        }

        /// <summary>
        /// 查询分页数据对象。
        /// </summary>
        /// <param name="data">查询条件对象。</param>
        /// <returns>分页数据对象。</returns>
        public ActionResult QueryModules(CriteriaData data)
        {
            return QueryPagingData(data, c => Service.QueryModules(c));
        }

        /// <summary>
        /// 删除一个或多个实体对象。
        /// </summary>
        /// <param name="data">实体对象 Id 数组。</param>
        /// <returns>删除结果。</returns>
        public ActionResult DeleteModules(string data)
        {
            return PostAction<string[]>(data, d => Service.DeleteModules(d));
        }

        /// <summary>
        /// 上移或下移模块数据。
        /// </summary>
        /// <param name="data">移动模块的id和方向，up-上移、down-下移。</param>
        /// <returns>移动结果。</returns>
        public ActionResult MoveModule(string data)
        {
            return PostAction<dynamic>(data, d =>
            {
                var id = (string)d.id;
                var direct = (string)d.direct;
                return Service.MoveModule(id, direct);
            });
        }
        #endregion

        #region Form
        /// <summary>
        /// 获取实体对象。
        /// </summary>
        /// <param name="id">实体 id。</param>
        /// <returns>实体对象。</returns>
        public ActionResult GetModule(string id)
        {
            return Json(Service.GetModule(id));
        }

        /// <summary>
        /// 保存实体对象。
        /// </summary>
        /// <param name="data">实体对象 JSON。</param>
        /// <returns>保存结果。</returns>
        public ActionResult SaveModule(string data)
        {
            return PostAction<dynamic>(data, d => Service.SaveModule(d));
        }
        #endregion
    }
}