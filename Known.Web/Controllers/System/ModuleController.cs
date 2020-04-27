using System.Linq;
using System.Web.Mvc;
using Known.Core;
using Known.Core.Services;

namespace Known.Web.Controllers
{
    public class ModuleController : ControllerBase
    {
        private ModuleService Service { get; } = new ModuleService();

        #region View
        public ActionResult GetModuleTree()
        {
            var modules = Service.GetModules();
            return JsonResult(modules.Select(m => m.ToTree()));
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

        [HttpPost]
        public ActionResult CopyModules(string data, string mid)
        {
            return PostAction<string[]>(data, d => Service.CopyModules(d, mid));
        }
        #endregion

        #region Form
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
    }
}