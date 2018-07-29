using System.IO;
using System.Web;
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
            var path = Path.Combine(HttpRuntime.AppDomainAppPath, "menu.json");
            var json = File.ReadAllText(path);
            return ApiResult.Success(json.FromJson<object>());
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
                    ViewType = ModuleViewType.SplitPageView
                };
            }

            return ApiResult.Success(module);
        }

        //class Menu
        //{
        //    public string id { get; set; }
        //    public string text { get; set; }
        //    public string url { get; set; }
        //    public string iconCls { get; set; }
        //    public List<Menu> children { get; set; }
        //}
    }
}