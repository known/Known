using System.Collections.Generic;
using System.Web.Mvc;
using Known.Extensions;
using Known.Platform;
using Known.Platform.Services;
using Known.Web.Extensions;
using Newtonsoft.Json;

namespace Known.Web.Controllers.Platform
{
    public class ModuleController : AuthorizeController
    {
        private ModuleService Service
        {
            get { return LoadService<ModuleService>(); }
        }

        public ActionResult QueryModules(string query, string isLoad)
        {
            var sortField = Request.Get<string>("sortField");
            var sortOrder = Request.Get<string>("sortOrder");
            var sorts = new List<string>();
            if (!string.IsNullOrWhiteSpace(sortField))
            {
                sorts.Add($"{sortField} {sortOrder}");
            }

            var criteria = new PagingCriteria
            {
                IsLoad = isLoad == "1",
                PageIndex = Request.Get<int>("pageIndex"),
                PageSize = Request.Get<int>("pageSize"),
                OrderBys = sorts.ToArray(),
                Parameter = FromJson(query)
            };

            var result = Service.QueryModules(criteria);
            return PageResult(result);
        }

        private dynamic FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            return JsonConvert.DeserializeObject<dynamic>(json);
        }

        public ActionResult GetTreeDatas()
        {
            var menus = new List<Menu>();
            var modules = Service.GetModules(true);
            if (modules != null && modules.Count > 0)
            {
                foreach (var item in modules)
                {
                    var menu = Menu.GetMenu(item);
                    menu.expanded = item.ParentId == "-1" || item.ParentId == "0";
                    menus.Add(menu);
                }
            }

            return JsonResult(menus);
        }

        public ActionResult SaveModule(string data)
        {
            var model = FromJson(data);
            var result = Service.SaveModule(model) as Result<Module>;
            return ExecuteResult(result);
        }

        public ActionResult DeleteModules(string[] data)
        {
            var modules = Service.GetModules(data);
            var result = Service.DeleteModules(modules);
            return ExecuteResult(result);
        }

        public ActionResult DropModule(string id, string pid)
        {
            var result = Service.DropModule(id, pid);
            return ExecuteResult(result);
        }
    }
}