using System.Web.Mvc;
using System.Web.Security;

namespace Known.Platform {
    public class HomeController : Controller {
        #region View
        [AllowAnonymous, Route("login")]
        public ActionResult Login() {
            return View();
        }

        public ActionResult Index() {
            return View();
        }
        #endregion

        #region Action
        [HttpPost]
        public ActionResult SignIn(string userName, string password, bool rememberMe) {
            FormsAuthentication.SetAuthCookie(userName, rememberMe);
            return Json(new { ok = true, msg = "登录成功！" });
        }

        [HttpPost]
        public ActionResult SignOut() {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
        #endregion
    }
}
