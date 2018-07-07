using System.Web.Mvc;
using System.Web.Security;
using Known.Platform;
using Known.Web.Filters;

namespace Known.Web.Controllers
{
    public class HomeController : BaseController
    {
        [LoginAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        [LoginAuthorize]
        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult Login(string backUrl)
        {
            ViewBag.BackUrl = backUrl;
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignIn(string userName, string password, string backUrl)
        {
            var appId = Config.AppSetting("SystemId");
            userName = userName.ToLower();
            var result = Api.Get<ApiResult>("/api/user/signin", new { appId, userName, password });
            if (result.Status == 1)
                return ErrorResult(result.Message);

            FormsAuthentication.SetAuthCookie(userName, true);
            CurrentUser = result.Data as User;

            if (string.IsNullOrEmpty(backUrl))
                backUrl = FormsAuthentication.DefaultUrl;

            return SuccessResult("登录成功，正在跳转页面......", backUrl);
        }

        [HttpPost]
        [LoginAuthorize]
        public void SignOut()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
        }
    }
}