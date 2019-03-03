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
            userName = userName.ToLower();
            var result = PlatformService.SignIn(userName, password);
            if (!result.IsValid)
                return ErrorResult(result.Message);

            FormsAuthentication.SetAuthCookie(userName, true);
            CurrentUser = result.Data;

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
    }
}