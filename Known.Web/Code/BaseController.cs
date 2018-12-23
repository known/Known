using System.Web.Mvc;
using Known.Drawing;
using Known.Extensions;
using Known.Log;
using Known.Platform;
using Known.Web.Extensions;

namespace Known.Web
{
    public class BaseController : AsyncController
    {
        protected ILogger log = new FileLogger();

        public BaseController()
        {
            ViewBag.SystemName = Setting.Instance.AppName;
        }

        public ActionResult Captcha()
        {
            var code = string.Empty;
            var bytes = ImageHelper.CreateCaptcha(4, out code);
            Session.SetValue("CaptchaCode", code);
            return File(bytes, MimeTypes.ImageJpeg);
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

        public Context Context
        {
            get { return Context.Create(UserName); }
        }

        public User CurrentUser
        {
            get
            {
                if (!(Session["CurrentUser"] is User user))
                {
                    user = PltApiHelper.GetUser(UserName);
                    Session["CurrentUser"] = user;
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

        public ApiClient Api
        {
            get { return GetApiClient(); }
        }

        public ApiClient GetApiClient(string apiId = null)
        {
            var baseUrl = PltApiHelper.GetApiBaseUrl(apiId);
            if (!IsAuthenticated || CurrentUser == null)
                return new ApiClient(baseUrl);

            return new ApiClient(baseUrl, CurrentUser.UserName, CurrentUser.Password);
        }

        public ActionResult ErrorResult(string message, object data = null)
        {
            return JsonResult(Result.Error(message, data));
        }

        public ActionResult SuccessResult(string message, object data = null)
        {
            return JsonResult(Result.Success(message, data));
        }

        public ActionResult JsonResult(object data)
        {
            return Content(data.ToJson(), MimeTypes.ApplicationJson);
        }

        public ActionResult PageResult(PagingResult result)
        {
            return JsonResult(new
            {
                total = result.TotalCount,
                data = result.PageData
            });
        }

        public ActionResult ExecuteResult(Result result)
        {
            if (!result.IsValid)
                return ErrorResult(result.Message);

            return SuccessResult(result.Message);
        }
    }
}