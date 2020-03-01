using System.Security.Principal;
using System.Web.SessionState;
using Known.Extensions;

namespace Known.Web.Mvc
{
    /// <summary>
    /// 控制器抽象类。
    /// </summary>
    public abstract class Controller
    {
        internal ControllerContext InnerContext { get; set; }

        /// <summary>
        /// 取得Http请求的用户登录信息。
        /// </summary>
        public IPrincipal User
        {
            get { return InnerContext.HttpContext.User; }
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
            get { return InnerContext.HttpContext.Session; }
        }

        /// <summary>
        /// 重新定向到新指定的url。
        /// </summary>
        /// <param name="url">新url。</param>
        /// <returns>新页面。</returns>
        protected ActionResult Redirect(string url)
        {
            InnerContext.HttpContext.Response.Redirect(url);
            return ActionResult.Empty;
        }

        /// <summary>
        /// 返回内容结果。
        /// </summary>
        /// <param name="content">内容字符串。</param>
        /// <param name="mimeType">内容类型。</param>
        /// <returns>字符串内容。</returns>
        protected ActionResult Content(string content, string mimeType = null)
        {
            return new ContentResult(InnerContext, content, mimeType);
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

        /// <summary>
        /// 返回页面视图。
        /// </summary>
        /// <returns>页面视图。</returns>
        protected ActionResult View()
        {
            return new ViewResult(InnerContext);
        }

        /// <summary>
        /// 返回部分视图。
        /// </summary>
        /// <returns>部分视图。</returns>
        protected ActionResult Partial(string viewName = null)
        {
            if (!string.IsNullOrWhiteSpace(viewName))
            {
                InnerContext.ActionName = viewName.Replace("/", ".");
            }
            return new ViewResult(InnerContext, true);
        }

        /// <summary>
        /// 返回文件结果。
        /// </summary>
        /// <param name="content">文件内容。</param>
        /// <param name="fileName">文件名。</param>
        /// <param name="mimeType">文件类型。</param>
        /// <returns>文件结果。</returns>
        protected ActionResult File(byte[] content, string fileName, string mimeType = null)
        {
            return new FileResult(InnerContext, content, fileName, mimeType);
        }
    }
}
