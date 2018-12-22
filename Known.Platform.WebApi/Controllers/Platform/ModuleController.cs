using System.Collections.Generic;
using Known.Platform.Services;
using Known.Platform.WebApi.Models;
using Known.Web;

namespace Known.Platform.WebApi.Controllers.Platform
{
    public class ModuleController : WebApiController
    {
        private ModuleService Service
        {
            get { return LoadService<ModuleService>(); }
        }

        public ApiResult QueryModules(PagingCriteria criteria)
        {
            var pr = Service.QueryModules(criteria);
            return ApiResult.ToPageData(pr);
        }

        public ApiResult GetTreeDatas()
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

            return ApiResult.ToData(menus);
        }

        public ApiResult GetModule(string mid)
        {
            var module = Service.GetModule(mid);
            return ApiResult.ToData(module);
        }

        public ApiResult SaveModule(dynamic model)
        {
            var result = Service.SaveModule(model);
            return ApiResult.Result(result);
        }

        public ApiResult DeleteModules(string[] data)
        {
            var modules = Service.GetModules(data);
            var result = Service.DeleteModules(modules);
            return ApiResult.Result(result);
        }

        public ApiResult DropModule(string id, string pid)
        {
            var result = Service.DropModule(id, pid);
            return ApiResult.Result(result);
        }
    }
}