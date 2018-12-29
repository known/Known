using System;
using System.Collections.Generic;

namespace Known
{
    public sealed class ServiceUtils
    {
        public static object Execute(string userName, string module, string method, object parameter)
        {
            var service = GetService(userName, module);
            if (service == null)
                throw new Exception($"暂未实现{module}模块！");

            var func = service.GetType().GetMethod(method);
            if (func == null)
                throw new Exception($"{module}模块暂未实现{method}方法！");

            return func.Invoke(service, new object[] { parameter });
        }

        public static object Execute(string userName, string module, string method, Dictionary<string, object> parameters)
        {
            var service = GetService(userName, module);
            if (service == null)
                throw new Exception($"暂未实现{module}模块！");

            var func = service.GetType().GetMethod(method);
            if (func == null)
                throw new Exception($"{module}模块暂未实现{method}方法！");

            var paramInfos = func.GetParameters();
            if (paramInfos == null || paramInfos.Length == 0)
                return func.Invoke(service, null);

            var parms = new List<object>();
            foreach (var item in paramInfos)
            {
                if (parameters.ContainsKey(item.Name))
                    parms.Add(parameters[item.Name]);
                else
                    parms.Add(null);
            }

            return func.Invoke(service, parms.ToArray());
        }

        private static object GetService(string userName, string module)
        {
            var service = Container.Resolve($"{module}Service");
            if (service != null)
            {
                ((ServiceBase)service).Context.UserName = userName;
            }
            return service;
        }
    }
}
