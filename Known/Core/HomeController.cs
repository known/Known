using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Known.Web;

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
            Platform.SignOut(UserName);
            FormsAuthentication.SignOut();
            return SuccessResult("成功退出！");
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
        public ActionResult GetTodoLists()
        {
            var data = new CriteriaData();
            return QueryPagingData(data, c => Platform.GetTodoLists(c));
        }

        public ActionResult GetCompanyNews()
        {
            var data = new CriteriaData();
            return QueryPagingData(data, c => Platform.GetCompanyNews(c));
        }

        public ActionResult GetShortCuts()
        {
            return null;
        }
        #endregion

        #region UserInfo
        public ActionResult GetUserInfo()
        {
            return JsonResult(Platform.GetUserInfo(UserName));
        }

        [HttpPost]
        public ActionResult SaveUserInfo(string data)
        {
            return PostAction<dynamic>(data, d => Platform.SaveUserInfo(d));
        }

        [HttpPost]
        public ActionResult UpdatePassword(string oldPassword, string password, string repassword)
        {
            return JsonResult(Platform.UpdatePassword(UserName, oldPassword, password, repassword));
        }
        #endregion
    }
}
