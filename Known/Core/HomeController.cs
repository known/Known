using System.Collections.Generic;
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

        public ActionResult GetTopMenus()
        {
            var tops = new List<object>();
            tops.Add(new { mark = "1", text = "开发框架", icon = "&#xe614;", href = "" });
            return JsonResult(new { data = tops });
        }

        public ActionResult GetLeftMenus(string pid)
        {
            var tops = new List<object>();
            tops.Add(new { 
                text = "系统管理", icon = "&#xe614;", 
                subset = new List<object>
                {
                    new {text="模块管理",icon="",href="/System/ModuleView"},
                    new {text="角色管理",icon="",href="/System/RoleView"},
                    new {text="用户管理",icon="",href="/System/UserView"}
                }
            });
            return JsonResult(new { data = tops });

            //var menus = Platform.GetUserMenus(UserName);
            //return JsonResult(menus.Select(m => new
            //{
            //    id = m.Id,
            //    pid = m.ParentId,
            //    code = m.Code,
            //    text = m.Name,
            //    iconCls = m.Icon,
            //    url = m.Url
            //}));
        }
        #endregion
    }
}
