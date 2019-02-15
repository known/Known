using System.Web.Http;
using Known.Platform;
using Known.Web;

namespace Known.WebApi.Controllers
{
    public class UserController : ApiControllerBase
    {
        [Route("signin")]
        public ApiResult SignIn(string userName, string password, string backUrl = null)
        {
            userName = userName.ToLower();
            var result = PlatformService.SignIn(userName, password);
            if (!result.IsValid)
                return ApiResult.Error(result.Message);

            return ApiResult.Success("登录成功，正在跳转页面......", new
            {
                result.Data.Token,
                backUrl
            });
        }

        [Route("signout")]
        public ApiResult SignOut()
        {
            PlatformService.SignOut(UserName);
            return ApiResult.Success("");
        }

        public ApiResult GetModules()
        {
            var menus = Menu.GetUserMenus(PlatformService, UserName);
            var codes = Code.GetCodes(PlatformService);
            return ApiResult.ToData(new { menus, codes });
        }
    }
}