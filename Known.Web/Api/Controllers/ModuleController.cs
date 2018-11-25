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
            var pid = (string)criteria.Parameters.pid;
            var key = (string)criteria.Parameters.key;

            var pr = Service.QueryModules(pid, key);
            return ApiResult.Success(new { total = pr.TotalCount, data = pr.PageData });
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

            return ApiResult.Success(menus);
        }

        public ApiResult GetModule(string mid)
        {
            var module = Service.GetModule(mid);
            return ApiResult.Success(module);
        }
    }
}