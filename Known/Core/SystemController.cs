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
        #endregion

        #region Module
        public ActionResult GetModuleTree()
        {
            var modules = Service.GetModules();
            return JsonResult(modules.Select(m => MenuInfo.ToTree(m)));
        }

        public ActionResult QueryModules(CriteriaData data)
        {
            return QueryPagingData(data, c => Service.QueryModules(c));
        }

        public ActionResult DeleteModules(string data)
        {
            return PostAction<string[]>(data, d => Service.DeleteModules(d));
        }

        public ActionResult GetModule(string id)
        {
            return JsonResult(Service.GetModule(id));
        }

        public ActionResult SaveModule(string data)
        {
            return PostAction<dynamic>(data, d => Service.SaveModule(d));
        }
        #endregion

        #region Role
        #endregion

        #region User
        #endregion
    }
}
