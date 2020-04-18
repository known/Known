using System.Linq;
using System.Web.Mvc;
using Known.Core.Services;
using Known.Web;

namespace Known.Core
{
    public class SystemController : Web.ControllerBase
    {
        private SystemService Service { get; } = new SystemService();

        #region View
        public ActionResult ModuleView()
        {
            return ViewResult();
        }

        public ActionResult RoleView()
        {
            return ViewResult();
        }

        public ActionResult UserView()
        {
            return ViewResult();
        }

        public ActionResult RightForm()
        {
            return PartialResult("Partials/RightForm");
        }
        #endregion

        #region Module
        public ActionResult GetModuleTree()
        {
            var modules = Service.GetModules();
            return JsonResult(modules.Select(m => MenuInfo.ToTree(m)));
        }

        [HttpPost]
        public ActionResult QueryModules(CriteriaData data)
        {
            return QueryPagingData(data, c => Service.QueryModules(c));
        }

        [HttpPost]
        public ActionResult DeleteModules(string data)
        {
            return PostAction<string[]>(data, d => Service.DeleteModules(d));
        }

        public ActionResult GetModule(string id)
        {
            return JsonResult(Service.GetModule(id));
        }

        [HttpPost]
        public ActionResult SaveModule(string data)
        {
            return PostAction<dynamic>(data, d => Service.SaveModule(d));
        }
        #endregion

        #region Role
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

        #region User
        [HttpPost]
        public ActionResult QueryUsers(CriteriaData data)
        {
            return QueryPagingData(data, c => Service.QueryUsers(c));
        }

        [HttpPost]
        public ActionResult DeleteUsers(string data)
        {
            return PostAction<string[]>(data, d => Service.DeleteUsers(d));
        }

        [HttpPost]
        public ActionResult SetUserPwds(string data)
        {
            return PostAction<string[]>(data, d => Service.SetUserPwds(d));
        }

        [HttpPost]
        public ActionResult EnableUsers(string data, int enable)
        {
            return PostAction<string[]>(data, d => Service.EnableUsers(d, enable));
        }

        public ActionResult GetUser(string id)
        {
            return JsonResult(Service.GetUser(id));
        }

        [HttpPost]
        public ActionResult SaveUser(string data)
        {
            return PostAction<dynamic>(data, d => Service.SaveUser(d));
        }

        public ActionResult GetUserRoles(string userId)
        {
            var roles = Service.GetRoles().Select(r => new { value = r.Id, title = r.Name });
            var value = Service.GetUserRoles(userId);
            return JsonResult(new { roles, value });
        }

        [HttpPost]
        public ActionResult SaveUserRoles(string userId, string data)
        {
            return null;
        }
        #endregion
    }
}
