using System.Web.Mvc;
using Known.Drawing;
using Known.Platform;
using Known.Web.Extensions;

namespace Known.Web
{
    /// <summary>
    /// 控制器基类。
    /// </summary>
    public class BaseController : AsyncController
    {
        /// <summary>
        /// 构造函数，创建一个控制器实例。
        /// </summary>
        public BaseController()
        {
            ViewBag.SystemName = Config.AppSetting("SystemName");
        }

        /// <summary>
        /// 获取当前验证码图片。
        /// </summary>
        /// <returns>验证码图片。</returns>
        public ActionResult Captcha()
        {
            var code = string.Empty;
            var bytes = ImageHelper.CreateCaptcha(4, out code);
            Session.SetValue("CaptchaCode", code);
            return File(bytes, "image/jpeg");
        }

        /// <summary>
        /// 取得当前验证码字符串。
        /// </summary>
        public string CaptchaCode
        {
            get { return Session.GetValue<string>("CaptchaCode"); }
        }

        private ApiClient api;

        /// <summary>
        /// 取得当前登录用户认证的Api客户端。
        /// </summary>
        public ApiClient Api
        {
            get
            {
                if (api == null)
                {
                    if (IsAuthenticated)
                        api = new ApiClient(CurrentUser.Token);
                    else
                        api = new ApiClient();
                }

                return api;
            }
        }

        /// <summary>
        /// 取得或设置当前用户。
        /// </summary>
        public User CurrentUser
        {
            get { return Session.GetValue<User>("CurrentUser"); }
            set { Session.SetValue("CurrentUser", value); }
        }

        /// <summary>
        /// 取得当前登录的用户账号。
        /// </summary>
        protected string UserName
        {
            get { return User.Identity.Name; }
        }

        /// <summary>
        /// 取得当前用户身份是否已认证。
        /// </summary>
        protected bool IsAuthenticated
        {
            get { return User.Identity.IsAuthenticated; }
        }

        /// <summary>
        /// 返回错误结果。
        /// </summary>
        /// <param name="message">错误消息。</param>
        /// <returns>错误结果。</returns>
        public ActionResult ErrorResult(string message) => JsonResult(Result.Error(message));

        /// <summary>
        /// 返回带错误数据的错误结果。
        /// </summary>
        /// <typeparam name="T">错误数据类型。</typeparam>
        /// <param name="message">错误消息。</param>
        /// <param name="data">错误数据。</param>
        /// <returns>错误结果。</returns>
        public ActionResult ErrorResult<T>(string message, T data) => JsonResult(Result.Error(message, data));

        /// <summary>
        /// 返回成功结果。
        /// </summary>
        /// <param name="message">成功消息。</param>
        /// <returns>成功结果。</returns>
        public ActionResult SuccessResult(string message) => JsonResult(Result.Success(message));

        /// <summary>
        /// 返回带成功数据的成功结果。
        /// </summary>
        /// <typeparam name="T">成功数据类型。</typeparam>
        /// <param name="message">成功消息。</param>
        /// <param name="data">成功数据。</param>
        /// <returns>成功结果。</returns>
        public ActionResult SuccessResult<T>(string message, T data) => JsonResult(Result.Success(message, data));

        /// <summary>
        /// 返回JSON数据结果。
        /// </summary>
        /// <param name="data">要序列化成JSON的数据对象。</param>
        /// <returns>JSON数据结果。</returns>
        public ActionResult JsonResult(object data) => Json(data, JsonRequestBehavior.AllowGet);
    }
}