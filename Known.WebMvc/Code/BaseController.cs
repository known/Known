using System.Web.Mvc;
using Known.Drawing;
using Known.Extensions;
using Known.Log;
using Known.Platform;
using Known.Web;
using Known.Web.Extensions;

namespace Known.WebMvc
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
            var bytes = ImageHelper.CreateCaptcha(4, out string code);
            Session.SetValue("CaptchaCode", code);
            return File(bytes, MimeTypes.ImageJpeg);
        }

        public ActionResult Partial(string name, dynamic model)
        {
            if (name.Contains("/"))
                return PartialView(name, model);

            return PartialView($"Partials/{name}", model);
        }

        protected Context Context
        {
            get { return Context.Create(UserName); }
        }

        protected string CaptchaCode
        {
            get { return Session.GetValue<string>("CaptchaCode"); }
        }

        protected User CurrentUser
        {
            get
            {
                if (!(Session["CurrentUser"] is User user))
                {
                    var service = ObjectFactory.Create<PlatformService>();
                    user = service.GetUser(UserName);
                    Session["CurrentUser"] = user;
                }
                return user;
            }
            set { Session["CurrentUser"] = value; }
        }

        protected string UserName
        {
            get { return User.Identity.Name; }
        }

        protected bool IsAuthenticated
        {
            get { return User.Identity.IsAuthenticated; }
        }

        protected PlatformService PlatformService
        {
            get { return ObjectFactory.Create<PlatformService>(); }
        }

        protected ApiClient GetApiClient(string apiId = null)
        {
            var baseUrl = PlatformService.GetApiBaseUrl(apiId);
            return GetBaseApiClient(baseUrl);
        }

        private ApiClient GetBaseApiClient(string baseUrl = null)
        {
            if (!IsAuthenticated || CurrentUser == null)
                return new ApiClient(baseUrl);

            return new ApiClient(baseUrl, CurrentUser.UserName, CurrentUser.Password);
        }

        protected ActionResult ErrorResult(string message, object data = null)
        {
            return JsonResult(Result.Error(message, data));
        }

        protected ActionResult SuccessResult(string message, object data = null)
        {
            return JsonResult(Result.Success(message, data));
        }

        protected ActionResult JsonResult(object data)
        {
            return Content(data.ToJson(), MimeTypes.ApplicationJson);
        }

        protected ActionResult PageResult(PagingResult result)
        {
            return JsonResult(new
            {
                total = result.TotalCount,
                data = result.PageData
            });
        }

        protected ActionResult ExecuteResult(Result result)
        {
            if (!result.IsValid)
                return ErrorResult(result.Message);

            return SuccessResult(result.Message);
        }
    }
}