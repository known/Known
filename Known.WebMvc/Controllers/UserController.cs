using System.Web.Mvc;
using System.Web.Security;
using Known.Platform;
using Known.WebMvc.Filters;

namespace Known.WebMvc.Controllers
{
    public class UserController : BaseController
    {
        [Route("signin")]
        public ActionResult SignIn(string userName, string password, string backUrl)
        {
            var result = PlatformService.SignIn(userName, password);
            if (!result.IsValid)
                return ErrorResult(result.Message);

            CurrentUser = result.Data;
            FormsAuthentication.SetAuthCookie(CurrentUser.UserName, true);

            if (string.IsNullOrEmpty(backUrl))
                backUrl = FormsAuthentication.DefaultUrl;

            return SuccessResult("登录成功，正在跳转页面......", backUrl);
        }

        [Route("signout")]
        [LoginAuthorize]
        public void SignOut()
        {
            PlatformService.SignOut(UserName);
            FormsAuthentication.SignOut();
            Session.Clear();
        }

        [LoginAuthorize]
        public ActionResult GetModules()
        {
            var menus = Menu.GetUserMenus(PlatformService, UserName);
            var codes = Code.GetCodes(PlatformService);
            return JsonResult(new { menus, codes });
        }

        [LoginAuthorize]
        public ActionResult GetCodes()
        {
            var codes = Code.GetCodes(PlatformService);
            return JsonResult(codes);
        }

        [LoginAuthorize]
        public ActionResult GetUserInfo()
        {
            return JsonResult(CurrentUser);
        }
    }
}