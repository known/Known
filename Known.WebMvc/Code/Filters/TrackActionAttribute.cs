using System.Web.Mvc;

namespace Known.WebMvc.Filters
{
    public class TrackActionAttribute : ActionFilterAttribute
    {
        //private VisitLog log = new VisitLog();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //log = filterContext.HttpContext.GetVisitLog();
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            //log.FinishTime = DateTime.Now;
            //ServiceFactory.Load<LogService>().AddVisitLog(log);
        }
    }
}