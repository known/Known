using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;

namespace Known.Web.Mvc
{
    class ClassHelper
    {
        private static readonly Dictionary<string, Type> caches = new Dictionary<string, Type>();
        private static readonly object lockObj = new object();

        public static Type GetController(string controllerName)
        {
            var controllers = GetControllers();
            if (!controllers.ContainsKey(controllerName))
                return null;

            return controllers[controllerName];
        }

        private static Dictionary<string, Type> GetControllers()
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

            return caches;
        }
    }
}
