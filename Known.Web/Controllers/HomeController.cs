using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace Known.Web.Controllers
{
    public class HomeController : ControllerBase
    {
        private HomeService Service => new HomeService();

        #region View
        [AllowAnonymous, Route("register")]
        public ActionResult Register()
        {
            return ViewResult();
        }

        [AllowAnonymous, Route("login")]
        public ActionResult Login()
        {
            return ViewResult();
        }

        public ActionResult Index()
        {
            return ViewResult();
        }

        public ActionResult Welcome()
        {
            return ViewResult();
        }

        public ActionResult UserInfo()
        {
            return ViewResult();
        }

        public ActionResult NotFound()
        {
            return ViewResult();
        }

        public ActionResult Error()
        {
            return ViewResult();
        }
        #endregion

        #region Register
        #endregion

        #region Login
        [HttpPost, AllowAnonymous, Route("signin")]
        public ActionResult SignIn(string userName, string password, bool remember = false)
        {
            var result = Platform.SignIn(userName, password);
            if (!result.IsValid)
                return ErrorResult(result.Message);

            FormsAuthentication.SetAuthCookie(userName, remember);
            return SuccessResult("登录成功！", result.Data);
        }

        [HttpPost, Route("signout")]
        public ActionResult SignOut()
        {
            Platform.SignOut(UserName);
            Session.Clear();
            FormsAuthentication.SignOut();
            return SuccessResult("成功退出！");
        }

        public ActionResult GetUserData()
        {
            var user = CurrentUser;
            var data = Platform.GetUserMenus(UserName);
            var menus = data.Select(m => m.ToTree());
            var codes = Platform.GetCodes(user.CompNo);
            return JsonResult(new { user, menus, codes });
        }
        #endregion

        #region Utils
        public ActionResult RefreshCache()
        {
            return SuccessResult("刷新成功！");
        }
        #endregion

        #region Profile
        public ActionResult GetUserInfo()
        {
            return JsonResult(Platform.GetUserInfo(UserName));
        }

        [HttpPost]
        public ActionResult SaveUserInfo(string data)
        {
            return PostAction<UserInfo>(data, d => Platform.SaveUserInfo(d));
        }

        [HttpPost]
        public ActionResult UpdatePassword(string oldPassword, string password, string repassword)
        {
            var result = Platform.UpdatePassword(CurrentUser, oldPassword, password, repassword);
            return ValidateResult(result);
        }
        #endregion

        #region Welcome
        public ActionResult GetTodoLists()
        {
            var data = new CriteriaData();
            return QueryPagingData(data, c => Service.GetTodoLists(c));
        }

        public ActionResult GetCompanyNews()
        {
            var data = new CriteriaData();
            return QueryPagingData(data, c => Service.GetCompanyNews(c));
        }

        public ActionResult GetShortCuts()
        {
            return null;
        }
        #endregion
    }
}
