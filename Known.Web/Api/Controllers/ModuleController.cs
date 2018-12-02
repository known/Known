using System.Collections.Generic;
using Known.Platform.Services;

namespace Known.Web.Api.Controllers
{
    public class ModuleController : BaseApiController
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
    }
}