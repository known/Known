using System;
using System.Collections.Generic;
using System.Reflection;

namespace Known.Web.Mvc
{
    /// <summary>
    /// Action信息。
    /// </summary>
    public class ActionInfo
    {
        internal ActionInfo() { }

        private ActionInfo(Type controller, MethodInfo method)
        {
            Controller = controller;
            Method = method;
            Route = method.GetCustomAttribute<RouteAttribute>();
            Name = method.Name;
        }

        /// <summary>
        /// 取得Action名称。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 取得或设置Action的控制器类型。
        /// </summary>
        public Type Controller { get; set; }

        /// <summary>
        /// 取得或设置Action的方法信息。
        /// </summary>
        public MethodInfo Method { get; set; }

        /// <summary>
        /// 取得Action路由特性。
        /// </summary>
        public RouteAttribute Route { get; }

        /// <summary>
        /// 取得或设置请求方式（GET或POST）。
        /// </summary>
        public string RequestType { get; set; }

        /// <summary>
        /// 取得或设置路由参数字典。
        /// </summary>
        public Dictionary<string, object> QueryDatas { get; set; }

        /// <summary>
        /// 取得或设置POST数据字典。
        /// </summary>
        public Dictionary<string, string> FormDatas { get; set; }

        /// <summary>
        /// 判断方法或控制器是否使用指定类型的特性。
        /// </summary>
        /// <typeparam name="T">特性类型。</typeparam>
        /// <returns>使用返回True，否则返回False。</returns>
        public bool IsUseOf<T>() where T : Attribute
        {
            return Method.GetCustomAttributes(typeof(T), true).Length > 0 ||
                Controller.GetCustomAttributes(typeof(T), true).Length > 0;
        }

        internal static ActionInfo Create(AppInfo app, ControllerInfo controller, MethodInfo method)
        {
            var info = new ActionInfo(controller.Type, method);

            var attr = method.GetCustomAttribute<ModuleAttribute>();
            if (attr != null)
            {
                attr.Code = info.Name;
                attr.Parent = controller.Name;
                attr.Url = $"{controller.Name}/{info.Name}";
                app.AddModule(attr);
            }

            var btn = method.GetCustomAttribute<ToolbarAttribute>();
            if (btn != null)
            {
                btn.Page = controller.Type;
                btn.Url = $"{controller.Name}/{info.Name}";
                app.AddButton(btn);
            }

            return info;
        }
    }
}
