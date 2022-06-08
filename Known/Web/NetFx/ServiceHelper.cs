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

#if !NET6_0
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Known.Core;

namespace Known.Web
{
    class ServiceHelper
    {
        private static readonly Dictionary<string, ServiceInfo> caches = new Dictionary<string, ServiceInfo>();
        private static readonly Dictionary<string, ActionInfo> routes = new Dictionary<string, ActionInfo>();
        private static readonly object lockObj = new object();

        internal static void Init()
        {
            if (caches.Count == 0)
            {
                lock (lockObj)
                {
                    if (caches.Count == 0)
                    {
                        var files = Directory.GetFiles($@"{Config.RootPath}\bin", "*.dll");
                        var initTypes = new List<Type>();
                        var serviceTypes = new List<Type>();
                        foreach (var file in files)
                        {
                            var assembly = Assembly.LoadFrom(file);
                            if (!assembly.FullName.StartsWith("Known"))
                                continue;

                            var types = assembly.GetTypes();
                            foreach (var type in types)
                            {
                                if (typeof(IAppModule).IsAssignableFrom(type) && !type.IsAbstract)
                                {
                                    var module = Activator.CreateInstance(type) as IAppModule;
                                    module?.Initialize(Config.App);
                                }
                                else if (typeof(IService).IsAssignableFrom(type) && !type.IsAbstract)
                                {
                                    var info = new ServiceInfo(type);
                                    var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
                                    foreach (var method in methods)
                                    {
                                        var action = new ActionInfo(info, method);
                                        if (action.Route != null)
                                        {
                                            routes.Add(action.Route.Path, action);
                                        }
                                        info.Actions.Add(action);
                                    }
                                    caches.Add(info.Name.ToLower(), info);
                                }
                            }
                        }
                    }
                }
            }
        }

        internal static ActionInfo GetAction(string url)
        {
            if (routes.ContainsKey(url))
                return routes[url];

            var items = url.Split('/');
            var serviceName = "home";
            var actionName = "index";
            if (items.Length >= 3)
            {
                serviceName = $"{items[0]}.{items[1]}".ToLower();
                actionName = items[2].ToLower();
            }
            else if (items.Length >= 2)
            {
                serviceName = items[0].ToLower();
                actionName = items[1].ToLower();
            }

            if (!caches.ContainsKey(serviceName))
                return PrototypeHelper.GetAction(serviceName, actionName);

            var action = caches[serviceName].Actions.FirstOrDefault(a => a.Name.ToLower() == actionName);
            if (action == null)
                return PrototypeHelper.GetAction(serviceName, actionName);

            return action;
        }

        internal static ActionInfo GetAction(ActionContext context)
        {
            if (routes.ContainsKey(context.Url))
                return routes[context.Url];

            var actionName = string.IsNullOrEmpty(context.Action) ? "" : context.Action.ToLower();
            var serviceName = string.IsNullOrEmpty(context.Service) ? "" : context.Service.ToLower();
            if (!string.IsNullOrEmpty(context.App))
                serviceName = context.App.ToLower() + "." + serviceName;

            if (!caches.ContainsKey(serviceName))
                return PrototypeHelper.GetAction(serviceName, actionName);

            var action = caches[serviceName].Actions.FirstOrDefault(a => a.Name.ToLower() == actionName);
            if (action == null)
                return PrototypeHelper.GetAction(serviceName, actionName);

            return action;
        }
    }
}
#endif