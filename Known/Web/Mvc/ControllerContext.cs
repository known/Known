using System.Reflection;
using System.Web;

namespace Known.Web.Mvc
{
    public class ControllerContext
    {
        internal ControllerContext(HttpContext context, MethodInfo action)
        {
            HttpContext = context;
            Action = action;
        }

        public HttpContext HttpContext { get; }
        public MethodInfo Action { get; }
    }
}
