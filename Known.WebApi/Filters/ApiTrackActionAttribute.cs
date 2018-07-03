using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Known.Web;
using Known.Web.Extensions;
using Known.WebApi.Extensions;

namespace Known.WebApi.Filters
{
    public class ApiTrackActionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.IsUseAttributeOf<DoNotTrackAttribute>())
                return;

            var tracker = new RequestTracker();
            actionContext.Request.Properties["Tracker"] = tracker;
            tracker.Start(actionContext);
        }

        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            return base.OnActionExecutingAsync(actionContext, cancellationToken);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var tracker = actionExecutedContext.Request.GetPropertyValue<RequestTracker>("Tracker");
            if (tracker != null)
                tracker.Complete(actionExecutedContext);
        }

        public override Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            return base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
        }
    }
}