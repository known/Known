using System.Web.Mvc;
using Known.Drawing;
using Known.Extensions;
using Known.Log;
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
                        api = new ApiClient(CurrentUser.Token.ToString());
                    else
                        api = new ApiClient();
                }

                return api;
            }
        }

        public dynamic CurrentUser
        {
            get
            {
                var user = Session["CurrentUser"];
                if (user == null)
                {
                    var api = new ApiClient();
                    var result = api.Get<ApiResult>("/api/user/getuser", new { userName = UserName });
                    if (result.Status == 0)
                    {
                        user = result.Data;
                        Session["CurrentUser"] = user;
                    }
                }
                return user;
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