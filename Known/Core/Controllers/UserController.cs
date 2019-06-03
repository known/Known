using System.Web.Http;
using Known.Core.Services;
using Known.Extensions;
using Known.Web;
using Known.WebApi;

namespace Known.Core.Controllers
{
    /// <summary>
    /// 用户控制器类。
    /// </summary>
    public class UserController : ApiControllerBase
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
        /// <param name="backUrl">登录成功后，跳转的地址，默认空。</param>
        /// <returns>操作结果对象。</returns>
        [AllowAnonymous, Route("signin")]
        public object SignIn(string userName, string password, string backUrl = null)
        {
            userName = userName.ToLower();
            var result = PlatformService.SignIn(userName, password);
            if (!result.IsValid)
                return ApiResult.Error(result.Message);

            return ApiResult.Success("登录成功，正在跳转页面......", new
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
        [Route("signout")]
        public object SignOut()
        {
            PlatformService.SignOut(UserName);
            return ApiResult.Success("");
        }

        /// <summary>
        /// 获取当前用户菜单模块及系统代码集合。
        /// </summary>
        /// <returns>菜单模块及系统代码集合。</returns>
        public object GetModules()
        {
            var menus = Menu.GetUserMenus(PlatformService, UserName);
            var codes = Code.GetCodes(PlatformService);
            return new { menus, codes };
        }
        #endregion

        #region View
        /// <summary>
        /// 查询用户分页数据对象。
        /// </summary>
        /// <param name="data">查询条件对象。</param>
        /// <returns>分页数据对象。</returns>
        [HttpPost]
        public object QueryUsers(CriteriaData data)
        {
            var criteria = data.ToPagingCriteria();
            var result = Service.QueryUsers(criteria);
            return ApiResult.ToPageData(result);
        }

        /// <summary>
        /// 删除一个或多个用户数据。
        /// </summary>
        /// <param name="data">用户对象ID数组。</param>
        /// <returns>删除结果。</returns>
        [HttpPost]
        public object DeleteUsers([FromBody]string data)
        {
            var ids = data.FromJson<string[]>();
            var result = Service.DeleteUsers(ids);
            return ApiResult.Result(result);
        }
        #endregion

        #region Form
        /// <summary>
        /// 获取用户数据对象。
        /// </summary>
        /// <param name="id">用户 id。</param>
        /// <returns>数据对象。</returns>
        public object GetUser(string id)
        {
            return Service.GetUser(id);
        }

        /// <summary>
        /// 保存用户数据。
        /// </summary>
        /// <param name="data">用户数据 JSON。</param>
        /// <returns>保存结果。</returns>
        [HttpPost]
        public object SaveUser([FromBody]string data)
        {
            var model = data.FromJson<dynamic>();
            var result = Service.SaveUser(model);
            return ApiResult.Result(result);
        }
        #endregion
    }
}