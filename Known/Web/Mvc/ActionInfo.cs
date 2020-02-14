using System;
using System.Collections.Generic;
using System.Reflection;
using Known.Core;

namespace Known.Web.Mvc
{
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

        public string Name { get; }
        public Type Controller { get; set; }
        public MethodInfo Method { get; set; }
        public RouteAttribute Route { get; }
        public Dictionary<string, object> Datas { get; set; }

        internal static ActionInfo Create(ControllerInfo controller, MethodInfo method)
        {
            var info = new ActionInfo(controller.Type, method);

            var attr = method.GetCustomAttribute<ModuleAttribute>();
            if (attr != null)
            {
                attr.Code = info.Name;
                attr.Parent = controller.Name;
                AppInfo.Instance.AddModule(attr);
            }

            return info;
        }
    }
}
