using System.Web.Mvc;
using System.Web.Security;
using Known.Extensions;
using Known.Platform;
using Known.Web.Filters;

namespace Known.Web.Controllers
{
    public class UserController : BaseController
    {
        [Route("signin")]
        public ActionResult SignIn(string userName, string password, string backUrl)
        {
            userName = userName.ToLower();
            var result = PltApiHelper.SignIn(Api, userName, password);
            if (result.Status == 1)
                return ErrorResult(result.Message);

            FormsAuthentication.SetAuthCookie(userName, true);
            var json = SerializeExtension.ToJson(result.Data) as string;
            CurrentUser = json.FromJson<User>();

            if (string.IsNullOrEmpty(backUrl))
                backUrl = FormsAuthentication.DefaultUrl;

            return SuccessResult("登录成功，正在跳转页面......", backUrl);
        }

        [Route("signout")]
        [LoginAuthorize]
        public void SignOut()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
        }
    }
}