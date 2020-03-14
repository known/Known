using System.Web.Security;
using Known.Core;
using Known.Web.Mvc;
using Known.Web.Services;

namespace Known.Web.Controllers
{
    /// <summary>
    /// 用户控制器类。
    /// </summary>
    public class UserController : BaseController
    {
        private UserService Service
        {
            get { return LoadService<UserService>(); }
        }

        #region Platform
        /// <summary>
        /// 用户登录操作。
        /// </summary>
        /// <param name="userName">用户名。</param>
        /// <param name="password">登录密码。</param>
        /// <param name="rememberMe">是否记住，默认否。</param>
        /// <param name="backUrl">登录成功后，跳转的地址，默认空。</param>
        /// <returns>操作结果对象。</returns>
        [AllowAnonymous, Route("signin")]
        public ActionResult SignIn(string userName, string password, bool rememberMe = false, string backUrl = null)
        {
            var result = PlatformService.SignIn(userName, password);
            if (!result.IsValid)
                return ErrorResult(result.Message);

            FormsAuthentication.SetAuthCookie(userName, rememberMe);
            return SuccessResult("登录成功，正在跳转页面......", new
            {
                user = new
                {
                    result.Data.CompanyId,
                    result.Data.DepartmentId,
                    result.Data.UserName,
                    result.Data.Name,
                    result.Data.Email,
                    result.Data.Mobile,
                    result.Data.Phone,
                    result.Data.Token
                },
                backUrl
            });
        }

        /// <summary>
        /// 退出登录操作。
        /// </summary>
        /// <returns>操作结果对象。</returns>
        //[Route("signout")]
        public ActionResult SignOut()
        {
            PlatformService.SignOut(UserName);
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();
            return Redirect(FormsAuthentication.LoginUrl);
        }

        /// <summary>
        /// 获取当前用户菜单模块及系统代码集合。
        /// </summary>
        /// <returns>菜单模块及系统代码集合。</returns>
        public ActionResult GetModules()
        {
            var menus = Menu.GetUserMenus(PlatformService, UserName);
            var codes = Code.GetCodes(PlatformService);
            return Json(new { menus, codes });
        }

        /// <summary>
        /// 获取当前用户指定模块的权限数据。
        /// </summary>
        /// <param name="id">模块ID。</param>
        /// <returns>模块按钮和列表数据。</returns>
        public ActionResult GetModule(string id)
        {
            var data = PlatformService.GetUserModule(UserName, id);
            return Json(data);
        }
        #endregion

        #region View
        /// <summary>
        /// 查询分页数据对象。
        /// </summary>
        /// <param name="data">查询条件对象。</param>
        /// <returns>分页数据对象。</returns>
        public ActionResult QueryUsers(CriteriaData data)
        {
            return QueryPagingData(data, c => Service.QueryUsers(c));
        }

        /// <summary>
        /// 删除一个或多个实体对象。
        /// </summary>
        /// <param name="data">实体对象 Id 数组。</param>
        /// <returns>删除结果。</returns>
        public ActionResult DeleteUsers(string data)
        {
            return PostAction<string[]>(data, d => Service.DeleteUsers(d));
        }
        #endregion

        #region Form
        /// <summary>
        /// 获取实体对象。
        /// </summary>
        /// <param name="id">实体 id。</param>
        /// <returns>实体对象。</returns>
        public ActionResult GetUser(string id)
        {
            return Json(Service.GetUser(id));
        }

        /// <summary>
        /// 保存实体对象。
        /// </summary>
        /// <param name="data">实体对象 JSON。</param>
        /// <returns>保存结果。</returns>
        public ActionResult SaveUser(string data)
        {
            return PostAction<dynamic>(data, d => Service.SaveUser(d));
        }
        #endregion
    }
}