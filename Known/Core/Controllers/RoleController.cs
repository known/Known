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
        /// 查询分页数据对象。
        /// </summary>
        /// <param name="criteria">查询条件对象。</param>
        /// <returns>分页查询结果。</returns>
        protected override PagingResult QueryDatas(PagingCriteria criteria)
        {
            return Service.QueryRoles(criteria);
        }

        /// <summary>
        /// 删除一个或多个实体对象。
        /// </summary>
        /// <param name="ids">实体对象 Id 数组。</param>
        /// <returns>删除结果。</returns>
        protected override Result DeleteDatas(string[] ids)
        {
            return Service.DeleteRoles(ids);
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
            return Service.GetRole(id);
        }

        /// <summary>
        /// 保存实体对象。
        /// </summary>
        /// <param name="model">实体对象。</param>
        /// <returns>保存结果。</returns>
        protected override Result SaveData(dynamic model)
        {
            return Service.SaveRole(model);
        }
        #endregion
    }
}