using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;

namespace Known.Web.Controllers
{
    /// <summary>
    /// 用户控制器。
    /// </summary>
    public class UserController : BaseController
    {
        /// <summary>
        /// 登录页面。
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
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
        /// 获取当前用户菜单数据。
        /// </summary>
        /// <returns>菜单数据。</returns>
        public ActionResult GetUserMenus()
        {
            var menus = new List<object>() {
                new {
                    title = "系统管理",
                    icon = "icon-computer",
                    herf = "",
                    children = new List<object>()
                    {
                        new { title = "模块管理", icon = "&#xe61c;", herf = "/Sys/Module" },
                        new { title = "角色管理", icon = "&#xe609;", herf = "/Sys/Role" }
                    }
                }
            };
            return JsonResult(menus);
        }

        [HttpPost]
        public ActionResult SignIn(string account, string password, bool rememberMe, string returnUrl)
        {
            account = account.ToLower();
            //var result = LoadBusiness<UserBusiness>().SignIn(account, password);
            //if (!result.IsValid)
            //    return ErrorResult(result.Message);

            FormsAuthentication.SetAuthCookie(account, rememberMe);
            UserToken = "";

            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = FormsAuthentication.DefaultUrl;

            return SuccessResult("登录成功，正在跳转页面......", returnUrl);
        }

        [HttpPost]
        public void SignOut()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
        }
    }
}