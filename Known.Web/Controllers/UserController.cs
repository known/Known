using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;
using Known.Extensions;
using Known.Platform;
using Known.Web.Filters;

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

        class menu
        {
            public string id { get; set; }
            public string text { get; set; }
            public string url { get; set; }
            public string iconCls { get; set; }
            public List<menu> children { get; set; }
        }
        /// <summary>
        /// 获取当前用户菜单数据。
        /// </summary>
        /// <returns>菜单数据。</returns>
        [LoginAuthorize]
        public ActionResult GetUserMenus()
        {
            var menus = System.IO.File.ReadAllText(Server.MapPath("~/menu.json")).FromJson<List<menu>>();
            return JsonResult(menus);
        }

        class result
        {
            public int status { get; set; }
            public string message { get; set; }
            public dynamic data { get; set; }
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
            var result = Api.Get<result>("/api/user/signin", new { appId, userName, password });
            if (result.status == 1)
                return ErrorResult(result.message);

            FormsAuthentication.SetAuthCookie(userName, true);
            CurrentUser = result.data as User;

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