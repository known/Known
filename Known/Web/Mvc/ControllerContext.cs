using System;
using System.Reflection;
using System.Web;

namespace Known.Web.Mvc
{
    public class ControllerContext
    {
        internal ControllerContext(HttpContext context, Type type, MethodInfo action)
        {
            HttpContext = context;
            Type = type;
            Action = action;

            ControllerName = type.Name.Replace("Controller", "");
            ActionName = action.Name;
        }

        public string ControllerName { get; }
        public string ActionName { get; }
        public HttpContext HttpContext { get; }
        public Type Type { get; }
        public MethodInfo Action { get; }
    }
}
