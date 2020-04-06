using System;
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
        public ActionResult GetTodoLists()
        {
            var cd = new CriteriaData();
            var pr = new PagingResult<object>
            {
                TotalCount = 33,
                PageData = new System.Collections.Generic.List<object>
                {
                    new {Oid="1",Name="请假审批",Qty=1},
                    new {Oid="2",Name="费用报销",Qty=2},
                    new {Oid="3",Name="出差审批",Qty=3}
                }
            };
            return QueryPagingData(cd, c => pr);
        }

        public ActionResult GetCompanyNews()
        {
            var cd = new CriteriaData();
            var pr = new PagingResult<object>
            {
                TotalCount = 55,
                PageData = new System.Collections.Generic.List<object>
                {
                    new {Oid="1",Title="公司新系统上线",CreateBy="管理员",CreateTime=DateTime.Now},
                    new {Oid="2",Title="关于放假通知",CreateBy="张三",CreateTime=DateTime.Now},
                    new {Oid="3",Title="关于员工福利通知",CreateBy="李四",CreateTime=DateTime.Now}
                }
            };
            return QueryPagingData(cd, c => pr);
        }

        public ActionResult GetShortCuts()
        {
            return null;
        }
        #endregion

        #region UserInfo
        #endregion
    }
}
