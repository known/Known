using System.Security.Principal;
using System.Web.SessionState;
using Known.Extensions;

namespace Known.Web.Mvc
{
    /// <summary>
    /// 控制器类。
    /// </summary>
    public abstract class Controller
    {
        internal ControllerContext Context { get; set; }

        /// <summary>
        /// 取得Http请求的用户登录信息。
        /// </summary>
        public IPrincipal User
        {
            get { return Context.HttpContext.User; }
        }

        /// <summary>
        /// 取得系统当前登录用户名。
        /// </summary>
        protected string UserName
        {
            get { return User.Identity.Name; }
        }

        /// <summary>
        /// 取得系统当前用户是否已验证。
        /// </summary>
        protected bool IsAuthenticated
        {
            get { return User.Identity.IsAuthenticated; }
        }

        /// <summary>
        /// 取得Http请求的Session对象。
        /// </summary>
        public HttpSessionState Session
        {
            get { return Context.HttpContext.Session; }
        }

        /// <summary>
        /// 重新定向到新指定的url。
        /// </summary>
        /// <param name="url">新url。</param>
        /// <returns></returns>
        protected ActionResult Redirect(string url)
        {
            Context.HttpContext.Response.Redirect(url);
            return ActionResult.Empty;
        }

        /// <summary>
        /// 返回内容结果。
        /// </summary>
        /// <param name="content">内容字符串。</param>
        /// <param name="mimeType">内容类型。</param>
        /// <returns></returns>
        protected ActionResult Content(string content, string mimeType = null)
        {
            return new ContentResult(Context, content, mimeType);
        }

        /// <summary>
        /// 返回JSON结果。
        /// </summary>
        /// <param name="data">返回的对象。</param>
        /// <returns>JSON。</returns>
        protected ActionResult Json(object data)
        {
            return Content(data.ToJson(), MimeTypes.ApplicationJson);
        }

        protected ActionResult View()
        {
            return new ViewResult(Context);
        }

        protected ActionResult File()
        {
            return new FileResult(Context);
        }
    }
}
