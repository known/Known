using System;
using System.Collections.Generic;
using System.Reflection;

namespace Known.Web.Mvc
{
    public class ActionInfo
    {
        internal ActionInfo() { }

        internal ActionInfo(Type controller, MethodInfo method)
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
    }
}
