using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace Known.Core
{
    public class HomeController : Web.ControllerBase
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

        public ActionResult UserInfo()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }
        #endregion

        #region Login
        [HttpPost, AllowAnonymous, Route("signin")]
        public ActionResult SignIn(string userName, string password, bool rememberMe = false)
        {
            var result = Platform.SignIn(userName, password);
            if (!result.IsValid)
                return ErrorResult(result.Message);

            FormsAuthentication.SetAuthCookie(userName, rememberMe);
            return SuccessResult("登录成功！", result.Data);
        }
        #endregion

        #region Index
        [HttpPost, Route("signout")]
        public ActionResult SignOut()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        public ActionResult GetMenus(string pid)
        {
            var menus = Platform.GetUserMenus(UserName, pid);
            return JsonResult(menus.Select(m => new
            {
                id = m.Id,
                pid = m.ParentId,
                code = m.Code,
                text = m.Name,
                icon = m.Icon,
                url = m.Url
            }));
        }
        #endregion

        #region Welcome
        #endregion

        #region UserInfo
        #endregion
    }
}
