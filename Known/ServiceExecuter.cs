using System;
using System.Collections.Generic;

namespace Known
{
    public class ServiceExecuter
    {
        public ServiceExecuter(string userName, string module)
            : this(userName, module, null)
        {
        }

        public ServiceExecuter(string userName, string module, string method)
        {
            UserName = userName;
            Module = module;
            Method = method;
        }

        public string UserName { get; }
        public string Module { get; }
        public string Method { get; set; }

        public object Execute(object parameter)
        {
            var service = GetService(UserName, Module);
            if (service == null)
                throw new Exception($"暂未实现{Module}模块！");

            var func = service.GetType().GetMethod(Method);
            if (func == null)
                throw new Exception($"{Module}模块暂未实现{Method}方法！");

            var paramInfos = func.GetParameters();
            if (paramInfos == null || paramInfos.Length == 0)
                return func.Invoke(service, null);

            return func.Invoke(service, new[] { parameter });
        }

        public object Execute(Dictionary<string, object> parameters)
        {
            var service = GetService(UserName, Module);
            if (service == null)
                throw new Exception($"暂未实现{Module}模块！");

            var func = service.GetType().GetMethod(Method);
            if (func == null)
                throw new Exception($"{Module}模块暂未实现{Method}方法！");

            var paramInfos = func.GetParameters();
            if (paramInfos == null || paramInfos.Length == 0)
                return func.Invoke(service, null);

            var parmeters = new List<object>();
            if (parameters == null || parameters.Count == 0)
                throw new Exception("参数不能为空！");

            foreach (var item in paramInfos)
            {
                if (parameters.ContainsKey(item.Name))
                    parmeters.Add(parameters[item.Name]);
                else
                    parmeters.Add(null);
            }

            return func.Invoke(service, parmeters.ToArray());
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
