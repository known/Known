using System.Web.Mvc;
using Known.Core.Services;

namespace Known.Web.Controllers
{
    public class ProfileController : ControllerBase
    {
        private ProfileService Service { get; } = new ProfileService();

        public ActionResult GetUserInfo()
        {
            return JsonResult(Platform.GetUserInfo(UserName));
        }

        [HttpPost]
        public ActionResult SaveUserInfo(string data)
        {
            return PostAction<dynamic>(data, d => Service.SaveUserInfo(d));
        }

        [HttpPost]
        public ActionResult UpdatePassword(string oldPassword, string password, string repassword)
        {
            return JsonResult(Service.UpdatePassword(CurrentUser, oldPassword, password, repassword));
        }
    }
}