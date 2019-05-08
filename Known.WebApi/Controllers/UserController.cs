using System.Web.Http;
using Known.Platform;
using Known.Web;

namespace Known.WebApi.Controllers
{
    public class UserController : ApiControllerBase
    {
        [AllowAnonymous, Route("signin")]
        public object SignIn(string userName, string password, string backUrl = null)
        {
            userName = userName.ToLower();
            var result = PlatformService.SignIn(userName, password);
            if (!result.IsValid)
                return ApiResult.Error(result.Message);

            return ApiResult.Success("登录成功，正在跳转页面......", new
            {
                user = new
                {
                    result.Data.CompanyId,
                    result.Data.DepartmentId,
                    result.Data.UserName,
                    result.Data.Name,
                    result.Data.Email,
                    result.Data.Mobile,
                    result.Data.Phone,
                    result.Data.Token
                },
                backUrl
            });
        }

        [Route("signout")]
        public object SignOut()
        {
            PlatformService.SignOut(UserName);
            return ApiResult.Success("");
        }

        public object GetModules()
        {
            var menus = Menu.GetUserMenus(PlatformService, UserName);
            var codes = Code.GetCodes(PlatformService);
            return new { menus, codes };
        }
    }
}