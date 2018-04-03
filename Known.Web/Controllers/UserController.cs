using System.Web.Mvc;
using System.Web.Security;

namespace Known.Web.Controllers
{
    public class UserController : BaseController
    {
        public ActionResult Login()
        {
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
        public ActionResult SignIn(string account, string password, bool rememberMe, string returnUrl)
        {
            account = account.ToLower();
            //var result = LoadBusiness<UserBusiness>().SignIn(account, password);
            //if (!result.IsValid)
            //    return ErrorResult(result.Message);

            FormsAuthentication.SetAuthCookie(account, rememberMe);

            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = FormsAuthentication.DefaultUrl;

            return SuccessResult("登录成功，正在跳转页面......", returnUrl);
        }

        [HttpPost]
        public void SignOut()
        {
            //LoadBusiness<UserBusiness>().SignOut(UserName);
            FormsAuthentication.SignOut();
            Session.Clear();
        }
    }
}