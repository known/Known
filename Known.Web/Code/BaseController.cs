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
            ViewBag.SystemName = Setting.Instance.SystemName;
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
                        api = new ApiClient(null, CurrentUser.UserName, CurrentUser.Password);
                    else
                        api = new ApiClient();
                }

                return api;
            }
        }

        public User CurrentUser
        {
            get
            {
                var user = Session["CurrentUser"];
                if (user == null)
                {
                    var api = new ApiClient();
                    user = api.Get<User>("/api/user/getuser", new { userName = UserName });
                    Session["CurrentUser"] = user;
                }
                return user as User;
            }
            set { Session["CurrentUser"] = value; }
        }

        public string UserName
        {
            get { return User.Identity.Name; }
        }

        public bool IsAuthenticated
        {
            get { return User.Identity.IsAuthenticated; }
        }

        public ActionResult ErrorResult(string message)
        {
            return JsonResult(Result.Error(message));
        }

        public ActionResult ErrorResult<T>(string message, T data)
        {
            return JsonResult(Result.Error(message, data));
        }

        public ActionResult SuccessResult(string message)
        {
            return JsonResult(Result.Success(message));
        }

        public ActionResult SuccessResult<T>(string message, T data)
        {
            return JsonResult(Result.Success(message, data));
        }

        public ActionResult JsonResult(object data)
        {
            return Content(data.ToJson(), MimeTypes.ApplicationJson);
        }
    }
}