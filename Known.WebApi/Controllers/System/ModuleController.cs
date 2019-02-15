using Known.Platform;
using Known.Web;

namespace Known.WebApi.Controllers.System
{
    public class ModuleController : ApiControllerBase
    {
        public ApiResult GetTreeDatas()
        {
            var menus = Menu.GetTreeMenus(PlatformService);
            return ApiResult.ToData(menus);
        }
    }
}