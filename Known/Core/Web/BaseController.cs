using System;
using Known.Extensions;
using Known.Web.Mvc;

namespace Known.Core.Web
{
    /// <summary>
    /// 业务控制器基类。
    /// </summary>
    public abstract class BaseController : Controller
    {
        /// <summary>
        /// 取得应用程序上下文对象。
        /// </summary>
        protected Context Context
        {
            get { return Context.Create(UserName); }
        }

        /// <summary>
        /// 取得平台服务对象。
        /// </summary>
        protected PlatformService PlatformService
        {
            get { return ObjectFactory.Create<PlatformService>(); }
        }

        /// <summary>
        /// 获取指定服务类型的对象。
        /// </summary>
        /// <typeparam name="T">服务类型。</typeparam>
        /// <returns>服务类型对象。</returns>
        protected T LoadService<T>() where T : ServiceBase
        {
            return ObjectFactory.CreateService<T>(Context);
        }

        protected ActionResult GridView()
        {
            return new GridViewResult(InnerContext);
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
