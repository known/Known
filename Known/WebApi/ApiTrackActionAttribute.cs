using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Known.Web;
using Known.Web.Extensions;

namespace Known.WebApi
{
    /// <summary>
    /// Api操作跟踪特性类。
    /// </summary>
    public class ApiTrackActionAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 操作方法调用前的触发动作。
        /// </summary>
        /// <param name="actionContext">执行操作上下文对象。</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.IsUseAttributeOf<DoNotTrackAttribute>())
                return;

            var tracker = new RequestTracker();
            actionContext.Request.Properties["Tracker"] = tracker;
            tracker.Start(actionContext);
        }

        /// <summary>
        /// 操作方法调用前的异步触发动作。
        /// </summary>
        /// <param name="actionContext">执行操作上下文对象。</param>
        /// <param name="cancellationToken">取消操作通知。</param>
        /// <returns>异步操作。</returns>
        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            return base.OnActionExecutingAsync(actionContext, cancellationToken);
        }

        /// <summary>
        /// 操作方法调用后的触发动作。
        /// </summary>
        /// <param name="actionExecutedContext">执行操作上下文对象。</param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var tracker = actionExecutedContext.Request.GetPropertyValue<RequestTracker>("Tracker");
            if (tracker != null)
                tracker.Complete(actionExecutedContext);
        }

        /// <summary>
        /// 操作方法调用后的异步触发动作。
        /// </summary>
        /// <param name="actionExecutedContext">执行操作上下文对象。</param>
        /// <param name="cancellationToken">取消操作通知。</param>
        /// <returns>异步操作。</returns>
        public override Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            return base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
        }
    }
}