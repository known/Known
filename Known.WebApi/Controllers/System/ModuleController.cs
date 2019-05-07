using System.Web.Http;
using Known.Core.Services;
using Known.Platform;
using Known.Web;

namespace Known.WebApi.Controllers.System
{
    public class ModuleController : ApiControllerBase
    {
        private ModuleService Service
        {
            get { return Container.Resolve<ModuleService>(); }
        }

        #region ModuleView
        [HttpPost]
        public object GetTreeDatas()
        {
            var menus = Menu.GetTreeMenus(PlatformService);
            return menus;
        }
        #endregion

        #region ModuleGrid
        [HttpPost]
        public object QueryModules(CriteriaData data)
        {
            var criteria = data.ToPagingCriteria();
            var result = Service.QueryModules(criteria);
            return ApiResult.ToPageData(result);
        }
        #endregion
    }
}