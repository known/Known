using System.Web.Http;
using Known.Platform;

namespace Known.Web.Api.Controllers
{
    public class UserController : BaseApiController
    {
        [HttpGet]
        [AllowAnonymous]
        public object SignIn(string appId, string userName, string password)
        {
            if (userName != "13")
                return ApiResult.Error("用户名不存在！");

            var user = new User
            {
                UserName = userName
            };
            return ApiResult.Success(user);
        }
    }
}