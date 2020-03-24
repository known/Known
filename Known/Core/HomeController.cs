using System.Web.Mvc;
using System.Web.Security;
using Known.Web;

namespace Known.Core
{
    public class HomeController : BaseController
    {
        #region View
        [AllowAnonymous, Route("login")]
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Welcome()
        {
            return View();
        }
        #endregion

        #region Action
        [HttpPost, AllowAnonymous, Route("signin")]
        public ActionResult SignIn(string userName, string password, bool rememberMe = true)
        {
            var result = Platform.SignIn(userName, password);
            if (!result.IsValid)
                return ErrorResult(result.Message);

            FormsAuthentication.SetAuthCookie(userName, rememberMe);
            return SuccessResult("登录成功！", result.Data);
        }

        [HttpPost, Route("signout")]
        public ActionResult SignOut()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
        #endregion
    }
}
