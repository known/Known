using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;

namespace Known.Web.Mvc
{
    class WebApp
    {
        private static readonly Dictionary<string, ControllerInfo> caches = new Dictionary<string, ControllerInfo>();
        private static readonly Dictionary<string, ActionInfo> routes = new Dictionary<string, ActionInfo>();
        private static readonly object lockObj = new object();

        public static void Init()
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
                                    var info = new ControllerInfo(type);
                                    var methods = type.GetMethods();
                                    foreach (var method in methods)
                                    {
                                        var action = new ActionInfo(type, method);
                                        if (action.Route != null)
                                        {
                                            routes.Add(action.Route.Name, action);
                                        }
                                        info.Actions.Add(action);
                                    }
                                    caches.Add(info.Name, info);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static ActionInfo GetAction(string url)
        {
            if (routes.ContainsKey(url))
                return routes[url];

            var action = new ActionInfo();
            var items = url.Split('/');
            var controllerName = items.Length > 0 ? items[0] : "Home";
            if (caches.ContainsKey(controllerName))
                action.Controller = caches[controllerName].Type;

            if (action.Controller != null)
            {
                var actionName = items.Length > 1 ? items[1] : "Index";
                action.Method = action.Controller.GetMethod(actionName);
            }

            var id = items.Length > 2 ? items[2] : string.Empty;
            if (!string.IsNullOrWhiteSpace(id))
            {
                action.Datas = new Dictionary<string, object>();
                action.Datas["id"] = id;
            }

            return action;
        }
    }
}
