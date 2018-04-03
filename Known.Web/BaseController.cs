using Known.Log;
using System.Web.Mvc;

namespace Known.Web
{
    public class BaseController : AsyncController
    {
        private Context context;
        public Context Context
        {
            get
            {
                if (context == null)
                {
                    var database = Config.GetDatabase();
                    var logger = new ConsoleLogger();
                    context = new Context(database, logger);
                }
                context.UserName = UserName;
                return context;
            }
        }

        protected string UserName
        {
            get { return User.Identity.Name; }
        }

        protected bool IsAuthenticated
        {
            get { return User.Identity.IsAuthenticated; }
        }

        protected T LoadBusiness<T>() where T : Business
        {
            return BusinessFactory.Create<T>(Context);
        }

        public ActionResult ErrorResult(string message) => JsonResult(Result.Error(message));
        public ActionResult ErrorResult<T>(string message, T data) => JsonResult(Result.Error(message, data));
        public ActionResult SuccessResult(string message) => JsonResult(Result.Success(message));
        public ActionResult SuccessResult<T>(string message, T data) => JsonResult(Result.Success(message, data));
        public ActionResult JsonResult(object data) => Json(data, JsonRequestBehavior.AllowGet);
    }
}