using System.Web;

namespace Known.Web.Mvc
{
    /// <summary>
    /// 控制器上下文。
    /// </summary>
    public class ControllerContext
    {
        internal ControllerContext(HttpContext context, ActionInfo action)
        {
            HttpContext = context;
            Action = action;

            ControllerName = action.Controller.Name.Replace("Controller", "");
            ActionName = action.Method.Name;
        }

        /// <summary>
        /// 取得Http请求上下文。
        /// </summary>
        public HttpContext HttpContext { get; }

        /// <summary>
        /// 取得路由信息。
        /// </summary>
        public ActionInfo Action { get; }

        /// <summary>
        /// 取得控制器名称。
        /// </summary>
        public string ControllerName { get; }

        /// <summary>
        /// 取得Action名称。
        /// </summary>
        public string ActionName { get; internal set; }
    }
}
