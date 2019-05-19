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
            get { return LoadService<ModuleService>(); }
        }

        #region View
        [HttpPost]
        public object GetTreeDatas()
        {
            return Menu.GetTreeMenus(PlatformService);
        }

        [HttpPost]
        public object QueryModules(CriteriaData data)
        {
            var criteria = data.ToPagingCriteria();
            var result = Service.QueryModules(criteria);
            return ApiResult.ToPageData(result);
        }

        [HttpPost]
        public object DeleteModules([FromBody]string data)
        {
            var ids = data.FromJson<string[]>();
            var result = Service.DeleteModules(ids);
            return ApiResult.Result(result);
        }

        [HttpPost]
        public object MoveModule([FromBody]string data)
        {
            var model = data.FromJson<dynamic>();
            var result = Service.MoveModule((string)model.id, (string)model.direct);
            return ApiResult.Result(result);
        }
        #endregion

        #region Form
        public object GetModule(string id)
        {
            return Service.GetModule(id);
        }

        [HttpPost]
        public object SaveModule([FromBody]string data)
        {
            var model = data.FromJson<dynamic>();
            var result = Service.SaveModule(model);
            return ApiResult.Result(result);
        }
        #endregion
    }
}