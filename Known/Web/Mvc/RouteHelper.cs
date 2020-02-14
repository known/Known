using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;

namespace Known.Web.Mvc
{
    class RouteHelper
    {
        private static readonly Dictionary<string, Type> caches = new Dictionary<string, Type>();
        private static readonly Dictionary<string, RouteInfo> routes = new Dictionary<string, RouteInfo>();
        private static readonly object lockObj = new object();

        public static RouteInfo GetRoute(string url)
        {
            InitControllers();
            InitRoutes();

            if (routes.ContainsKey(url))
                return routes[url];

            var route = new RouteInfo();
            var items = url.Split('/');
            var controllerName = items.Length > 0 ? items[0] : "Home";
            if (caches.ContainsKey(controllerName))
                route.Controller = caches[controllerName];

            if (route.Controller != null)
            {
                var actionName = items.Length > 1 ? items[1] : "Index";
                route.Action = route.Controller.GetMethod(actionName);
            }

            var id = items.Length > 2 ? items[2] : string.Empty;
            if (!string.IsNullOrWhiteSpace(id))
            {
                route.Datas = new Dictionary<string, object>();
                route.Datas["id"] = id;
            }

            return route;
        }

        private static void InitControllers()
        {
            if (caches.Count == 0)
            {
                lock (lockObj)
                {
                    if (caches.Count == 0)
                    {
                        var assemblies = BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToList();
                        foreach (var assembly in assemblies)
                        {
                            var types = assembly.GetExportedTypes();
                            if (types == null || types.Length == 0)
                                continue;

                            foreach (var type in types)
                            {
                                if (type.IsSubclassOf(typeof(Controller)))
                                {
                                    var name = type.Name.Replace("Controller", "");
                                    caches.Add(name, type);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void InitRoutes()
        {
            if (routes.Count == 0)
            {
                lock (lockObj)
                {
                    if (routes.Count == 0)
                    {
                        foreach (var item in caches.Values)
                        {
                            var methods = item.GetMethods();
                            foreach (var method in methods)
                            {
                                var attr = method.GetCustomAttribute<RouteAttribute>();
                                if (attr != null)
                                {
                                    routes.Add(attr.Name, new RouteInfo
                                    {
                                        Name = attr.Name,
                                        Controller = item,
                                        Action = method
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
