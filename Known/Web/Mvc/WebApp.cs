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
                                if (!type.IsSubclassOf(typeof(Controller)) || type.IsAbstract)
                                    continue;

                                var info = ControllerInfo.Create(type);
                                var methods = type.GetMethods();
                                foreach (var method in methods)
                                {
                                    if (method.ReturnType != typeof(ActionResult))
                                        continue;

                                    var action = ActionInfo.Create(info, method);
                                    if (action.Route != null)
                                    {
                                        routes.Add(action.Route.Name, action);
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

        public static Dictionary<string, ControllerInfo> GetControllers()
        {
            return caches;
        }

        public static ActionInfo GetAction(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                url = "Home/Index";

            if (routes.ContainsKey(url))
                return routes[url];

            var items = url.Split('/');
            var controller = GetController(items);
            if (controller == null)
                return null;

            var actionName = items.Length > 1 ? items[1] : "Index";
            var action = controller.GetAction(actionName);
            if (action == null)
                return null;

            var id = items.Length > 2 ? items[2] : string.Empty;
            if (!string.IsNullOrWhiteSpace(id))
                action.Datas = new Dictionary<string, object> { ["id"] = id };

            return action;
        }

        private static ControllerInfo GetController(string[] items)
        {
            var controllerName = items.Length > 0 ? items[0] : "Home";
            controllerName = controllerName.ToLower();

            if (!caches.ContainsKey(controllerName))
                return null;

            return caches[controllerName];
        }
    }
}
