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
                UserName = userName
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