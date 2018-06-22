using System.Web.Mvc;
using System.Web.Security;
using Known.Platform;
using Known.Web.Filters;

namespace Known.Web.Controllers
{
    /// <summary>
    /// 主页控制器。
    /// </summary>
    public class HomeController : BaseController
    {
        /// <summary>
        /// 默认首页。
        /// </summary>
        /// <returns></returns>
        [LoginAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 登录页面。
        /// </summary>
        /// <returns></returns>
        public ActionResult Login(string backUrl)
        {
            ViewBag.BackUrl = backUrl;
            return View();
        }

        /// <summary>
        /// 注册页面。
        /// </summary>
        /// <returns></returns>
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// 忘记密码页面。
        /// </summary>
        /// <returns></returns>
        public ActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// 登录验证。
        /// </summary>
        /// <param name="userName">登录账号。</param>
        /// <param name="password">登录密码。</param>
        /// <param name="backUrl">登录成功后跳转的地址。</param>
        /// <returns></returns>
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

        /// <summary>
        /// 退出系统。
        /// </summary>
        [HttpPost]
        [LoginAuthorize]
        public void SignOut()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
        }
    }
}