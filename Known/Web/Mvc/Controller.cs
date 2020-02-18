using System;
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
        /// <returns>新页面。</returns>
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
        /// <returns>字符串内容。</returns>
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

        /// <summary>
        /// 返回页面视图。
        /// </summary>
        /// <returns>页面视图。</returns>
        protected ActionResult View()
        {
            return new ViewResult(Context);
        }

        /// <summary>
        /// 返回部分视图。
        /// </summary>
        /// <returns>部分视图。</returns>
        protected ActionResult Partial(string viewName = null)
        {
            if (!string.IsNullOrWhiteSpace(viewName))
            {
                Context.ActionName = viewName.Replace("/", ".");
            }
            return new ViewResult(Context, true);
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
            return new FileResult(Context, content, fileName, mimeType);
        }

        /// <summary>
        /// 返回错误结果。
        /// </summary>
        /// <param name="message">错误消息。</param>
        /// <param name="data">错误对象。</param>
        /// <returns>错误结果</returns>
        protected ActionResult ErrorResult(string message, object data = null)
        {
            return Json(new { ok = false, message, data });
        }

        /// <summary>
        /// 返回成功结果。
        /// </summary>
        /// <param name="message">成功消息。</param>
        /// <param name="data">数据对象。</param>
        /// <returns>成功结果。</returns>
        protected ActionResult SuccessResult(string message, object data = null)
        {
            return Json(new { ok = true, message, data });
        }

        /// <summary>
        /// 返回验证结果。
        /// </summary>
        /// <param name="result">验证结果。</param>
        /// <returns>验证结果。</returns>
        protected ActionResult ValidateResult(Result result)
        {
            if (!result.IsValid)
                return ErrorResult(result.Message, result.Data);

            return SuccessResult(result.Message, result.Data);
        }

        /// <summary>
        /// 查询分页数据对象。
        /// </summary>
        /// <param name="data">查询条件对象。</param>
        /// <param name="func">查询方法。</param>
        /// <returns>分页数据对象。</returns>
        protected ActionResult QueryPagingData(CriteriaData data, Func<PagingCriteria, PagingResult> func)
        {
            var criteria = data.ToPagingCriteria();
            var result = func(criteria);
            return Json(new
            {
                total = result.TotalCount,
                data = result.PageData,
                summary = result.Summary
            });
        }

        /// <summary>
        /// 处理 POST 请求的操作。
        /// </summary>
        /// <typeparam name="T">POST 的数据类型。</typeparam>
        /// <param name="data">前端提交的 JSON 数据。</param>
        /// <param name="func">操作方法。</param>
        /// <returns>操作结果。</returns>
        protected ActionResult PostAction<T>(string data, Func<T, Result> func)
        {
            var obj = data.FromJson<T>();
            if (obj == null)
                return ErrorResult("不能提交空数据！");

            var result = func(obj);
            return ValidateResult(result);
        }
    }
}
