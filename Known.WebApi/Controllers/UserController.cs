using System.Web.Http;
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
    }
}