using System;
using System.Reflection;
using System.Web;

namespace Known.Web.Mvc
{
    /// <summary>
    /// 控制器上下文。
    /// </summary>
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

        /// <summary>
        /// 取得控制器名称。
        /// </summary>
        public string ControllerName { get; }

        /// <summary>
        /// 取得Http请求上下文。
        /// </summary>
        public HttpContext HttpContext { get; }

        /// <summary>
        /// 取得控制器类型。
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// 取得Action方法信息。
        /// </summary>
        public MethodInfo Action { get; }

        /// <summary>
        /// 取得Action名称。
        /// </summary>
        public string ActionName { get; internal set; }
    }
}
