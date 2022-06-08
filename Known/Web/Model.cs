/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * ------------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#if !NET35
using System.Threading.Tasks;
#endif

namespace Known.Web
{
    class ServiceInfo
    {
        internal ServiceInfo(Type type)
        {
            Type = type;
            Name = type.Name.Replace("Service", "");

            var asmNames = type.Assembly.ManifestModule.Name.Replace(".dll", "").Split('.');
            if (asmNames.Length > 1)
            {
                Name = asmNames.Last() + "." + Name;
            }

            Actions = new List<ActionInfo>();
        }

        internal string Name { get; }
        internal Type Type { get; }
        internal List<ActionInfo> Actions { get; }
    }

    class ActionInfo
    {
        internal ActionInfo(ServiceInfo service, MethodInfo method)
        {
            Service = service;
            Method = method;
            if (method != null)
            {
                Name = method.Name;
                Parameters = method.GetParameters();
#if !NET35
                IsTaskMethod = method.ReturnType == typeof(Task);
#endif
                //Route = method.GetCustomAttribute<RouteAttribute>();
                //IsAnonymous = method.GetCustomAttribute<AnonymousAttribute>() != null;
                Route = GetCustomAttribute<RouteAttribute>(method);
                IsAnonymous = GetCustomAttribute<AnonymousAttribute>(method) != null;
            }
            Datas = new Dictionary<string, string>();
        }

        internal string Name { get; }
        internal ServiceInfo Service { get; }
        internal MethodInfo Method { get; }
        internal bool IsTaskMethod { get; }
        internal ParameterInfo[] Parameters { get; }
        internal RouteAttribute Route { get; }
        internal bool IsAnonymous { get; }
        internal Dictionary<string, string> Datas { get; }
        internal string PrototypeName { get; set; }

        private T GetCustomAttribute<T>(MethodInfo method)
        {
            var attrs = method.GetCustomAttributes(true);
            if (attrs == null || attrs.Length == 0)
                return default;

            foreach (var item in attrs)
            {
                if (item is T attr)
                {
                    return attr;
                }
            }

            return default;
        }
    }
}
