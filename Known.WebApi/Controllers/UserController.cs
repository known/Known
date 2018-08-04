using System.Linq;
using System.Web.Http;
using Known.Extensions;
using Known.Platform;
using Known.Web;

namespace Known.WebApi.Controllers
{
    public class UserController : BaseApiController
    {
        [HttpGet]
        [AllowAnonymous]
        public ApiResult SignIn(string appId, string userName, string password)
        {
            if (userName != "known")
                return ApiResult.Error("用户名不存在！");

            var user = new User
            {
                UserName = userName,
                Name = "管理员",
                Token = "admin"
            };
            return ApiResult.Success(user);
        }

        [HttpGet]
        public ApiResult GetUser(string userName)
        {
            var user = new User
            {
                UserName = userName,
                Name = "管理员",
                Token = "admin"
            };
            return ApiResult.Success(user);
        }

        [HttpGet]
        public ApiResult GetMenus()
        {
            var modules = Context.Database.QueryList<Module>("select * from t_plt_modules");
            if (modules == null || modules.Count == 0)
                return ApiResult.Success();

            var menus = modules.Select(m => new
            {
                id = m.Id,
                code = m.Code,
                text = m.Name,
                pid = m.ParentId,
                iconCls = m.Icon,
                url = m.Url
            }).ToList();
            return ApiResult.Success(menus);

            //var path = Path.Combine(HttpRuntime.AppDomainAppPath, "menu.json");
            //var json = File.ReadAllText(path);
            //return ApiResult.Success(json.FromJson<object>());
        }

        [HttpGet]
        public ApiResult GetModule(string mid)
        {
            Module module = null;
            if (mid == "demo")
            {
                module = new Module
                {
                    Id = "demo",
                    Code = "Demo",
                    Name = "开发示例",
                    ViewType = ViewType.SplitPageView,
                    Extension = new { LeftPartialName = "Demo/DemoMenu" }.ToJson()
                };
            }

            return ApiResult.Success(module);
        }
    }
}