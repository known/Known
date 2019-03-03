using System.Web.Mvc;
using Known.Core.Services;
using Known.Platform;

namespace Known.WebMvc.Controllers.System
{
    public class ModuleController : AuthorizeController
    {
        private ModuleService Service
        {
            get { return Container.Resolve<ModuleService>(); }
        }

        #region View
        public ActionResult GetTreeDatas()
        {
            var menus = Menu.GetTreeMenus(PlatformService);
            return JsonResult(menus);
        }

        public ActionResult QueryModules(PagingCriteria criteria)
        {
            var result = Service.QueryModules(criteria);
            return PageResult(result);
        }
        #endregion

        #region Form
        #endregion
    }
}