using System.Web.Http;
using Known.Core.Services;
using Known.Extensions;
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
            return Menu.GetTreeMenus(PlatformService);
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

        [HttpPost]
        public ApiResult DeleteModules(string data)
        {
            var result = Service.DeleteModules(data);
            return ApiResult.Result(result);
        }
        #endregion

        #region ModuleForm
        public object GetModule(string id)
        {
            return Service.GetModule(id);
        }

        [HttpPost]
        public ApiResult SaveModule(string data)
        {
            var model = data.FromJson<dynamic>();
            var result = Service.SaveModule(model);
            return ApiResult.Result(result);
        }
        #endregion
    }
}