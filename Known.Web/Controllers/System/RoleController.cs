using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Known.Core.Services;

namespace Known.Web.Controllers
{
    public class RoleController : ControllerBase
    {
        private RoleService Service { get; } = new RoleService();

        #region View
        [HttpPost]
        public ActionResult QueryRoles(CriteriaData data)
        {
            return QueryPagingData(data, c => Service.QueryRoles(c));
        }

        [HttpPost]
        public ActionResult DeleteRoles(string data)
        {
            return PostAction<string[]>(data, d => Service.DeleteRoles(d));
        }
        #endregion

        #region Form
        public ActionResult GetRole(string id)
        {
            return JsonResult(Service.GetRole(id));
        }

        [HttpPost]
        public ActionResult SaveRole(string data)
        {
            return PostAction<dynamic>(data, d => Service.SaveRole(d));
        }
        #endregion

        #region RoleModule
        public ActionResult GetRoleModules(string roleId)
        {
            var modules = Service.GetRoleModules(roleId);
            return JsonResult(modules.Select(m => m.ToTree()));
        }

        public ActionResult SaveRoleModules(string roleId, string data)
        {
            return PostAction<List<string>>(data, d => Service.SaveRoleModules(roleId, d));
        }
        #endregion
    }
}