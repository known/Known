using System.Linq;
using System.Web.Mvc;

namespace Known.Web.Controllers
{
    public class DevelopController : ControllerBase
    {
        private DevelopService Service => new DevelopService();

        #region View
        public ActionResult ModuleView()
        {
            return ViewResult();
        }

        public ActionResult CodeView()
        {
            return ViewResult();
        }
        #endregion

        #region Module
        public ActionResult GetModuleTree()
        {
            var modules = Service.GetModules();
            return JsonResult(modules.Select(m => new
            {
                id = m.Id,
                pid = m.ParentId,
                name = m.Name,
                title = m.Name,
                icon = m.Icon,
                open = string.IsNullOrWhiteSpace(m.ParentId),
                module = m
            }));
        }

        [HttpPost]
        public ActionResult QueryModules(CriteriaData data)
        {
            return QueryPagingData(data, c => Service.QueryModules(c));
        }

        [HttpPost]
        public ActionResult CopyModules(string data, string mid)
        {
            return PostAction<string[]>(data, d => Service.CopyModules(d, mid));
        }

        [HttpPost]
        public ActionResult DeleteModules(string data)
        {
            return PostAction<string[]>(data, d => Service.DeleteModules(d));
        }

        [HttpPost]
        public ActionResult ExportModules(string data)
        {
            var ids = Utils.FromJson<string[]>(data);
            var bytes = Service.ExportModules(ids);
            return ExportResult(bytes, "Module.sql");
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

        #region Code
        public ActionResult GetModels()
        {
            var lists = Service.GetModels();
            return JsonResult(lists.Select(l => new
            {
                id = l.Code,
                name = l.Name,
                title = l.Name,
                pid = l.Category
            }));
        }

        public ActionResult GetModel(string code)
        {
            var result = Service.GetModel(code);
            return ValidateResult(result);
        }

        [HttpPost]
        public ActionResult SaveModel(string data)
        {
            var form = Utils.FromJson<DomainInfo>(data);
            var result = Service.SaveModel(form);
            return ValidateResult(result);
        }

        [HttpPost]
        public ActionResult DeleteModel(string code)
        {
            var result = Service.DeleteModel(code);
            return ValidateResult(result);
        }
        #endregion
    }
}
