using Known.Core;
using Known.Web.Mvc;
using Known.Web.Services;

namespace Known.Web.Controllers
{
    /// <summary>
    /// 角色管理控制器类。
    /// </summary>
    public class RoleController : BaseController
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
        public ActionResult QueryRoles(CriteriaData data)
        {
            return QueryPagingData(data, c => Service.QueryRoles(c));
        }

        [Toolbar(1, ToolbarType.Add)]
        public ActionResult AddRole()
        {
            return ActionResult.Empty;
        }

        [Toolbar(2, ToolbarType.Edit)]
        public ActionResult EditRole()
        {
            return ActionResult.Empty;
        }

        /// <summary>
        /// 删除一个或多个实体对象。
        /// </summary>
        /// <param name="data">实体对象 Id 数组。</param>
        /// <returns>删除结果。</returns>
        [Toolbar(3, ToolbarType.Remove)]
        public ActionResult DeleteRoles(string data)
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
        public ActionResult GetRole(string id)
        {
            return Json(Service.GetRole(id));
        }

        /// <summary>
        /// 保存实体对象。
        /// </summary>
        /// <param name="data">实体对象 JSON。</param>
        /// <returns>保存结果。</returns>
        [Toolbar(1, ToolbarType.Save, true)]
        public ActionResult SaveRole(string data)
        {
            return PostAction<dynamic>(data, d => Service.SaveRole(d));
        }
        #endregion
    }
}