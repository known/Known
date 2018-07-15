using System.Web.Mvc;
using Known.Drawing;
using Known.Extensions;
using Known.Log;
using Known.Platform;
using Known.Web.Extensions;

namespace Known.Web
{
    public class BaseController : AsyncController, IController
    {
        protected ILogger log = new FileLogger();

        public BaseController()
        {
            ViewBag.SystemName = Config.AppSetting("SystemName");
        }

        public ActionResult Captcha()
        {
            var code = string.Empty;
            var bytes = ImageHelper.CreateCaptcha(4, out code);
            Session.SetValue("CaptchaCode", code);
            return File(bytes, "image/jpeg");
        }

        public ActionResult Partial(string name, dynamic model)
        {
            if (name.Contains("/"))
                return PartialView(name, model);

            return PartialView($"Partials/{name}", model);
        }

        public string CaptchaCode
        {
            get { return Session.GetValue<string>("CaptchaCode"); }
        }

        private ApiClient api;
        public ApiClient Api
        {
            get
            {
                if (api == null)
                {
                    if (IsAuthenticated && CurrentUser != null)
                        api = new ApiClient(CurrentUser.Token);
                    else
                        api = new ApiClient();
                }

                return api;
            }
        }

        public User CurrentUser
        {
            get { return Session.GetValue<User>("CurrentUser"); }
            set { Session.SetValue("CurrentUser", value); }
        }

        public string UserName
        {
            get { return User.Identity.Name; }
        }

        public bool IsAuthenticated
        {
            get { return User.Identity.IsAuthenticated; }
        }

        public ActionResult ErrorResult(string message) => JsonResult(Result.Error(message));
        public ActionResult ErrorResult<T>(string message, T data) => JsonResult(Result.Error(message, data));
        public ActionResult SuccessResult(string message) => JsonResult(Result.Success(message));
        public ActionResult SuccessResult<T>(string message, T data) => JsonResult(Result.Success(message, data));
        public ActionResult JsonResult(object data) => Content(data.ToJson(), MimeTypes.ApplicationJson);
    }
}