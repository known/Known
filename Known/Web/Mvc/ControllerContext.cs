using System.Web;

namespace Known.Web.Mvc
{
    /// <summary>
    /// 控制器上下文。
    /// </summary>
    public class ControllerContext
    {
        internal ControllerContext(HttpContext context, RouteInfo route)
        {
            HttpContext = context;
            Route = route;

            ControllerName = route.Controller.Name.Replace("Controller", "");
            ActionName = route.Action.Name;
        }

        /// <summary>
        /// 取得Http请求上下文。
        /// </summary>
        public HttpContext HttpContext { get; }

        /// <summary>
        /// 取得路由信息。
        /// </summary>
        public RouteInfo Route { get; }

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
