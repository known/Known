using System.Collections.Generic;
using System.Web.Http;
using Known.Platform.Services;

namespace Known.Web.Api.Controllers
{
    public class ModuleController : BaseApiController
    {
        private ModuleService Service
        {
            get { return LoadService<ModuleService>(); }
        }

        [HttpGet]
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

            return ApiResult.Success(menus);
        }

        [HttpGet]
        public ApiResult GetModule(string mid)
        {
            var module = Service.GetModule(mid);
            return ApiResult.Success(module);
        }
    }
}