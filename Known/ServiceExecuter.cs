using System;
using System.Collections.Generic;

namespace Known
{
    public class ServiceExecuter
    {
        public ServiceExecuter(string userName, string module, string method)
        {
            UserName = userName;
            Module = module;
            Method = method;
        }

        public string UserName { get; }
        public string Module { get; }
        public string Method { get; }
        public object Parameter { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();

        public object Execute()
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

            var parameters = new List<object>();
            if (Parameter != null)
            {
                parameters.Add(Parameter);
            }
            else
            {
                if (Parameters == null || Parameters.Count == 0)
                    throw new Exception("参数不能为空！");

                foreach (var item in paramInfos)
                {
                    if (Parameters.ContainsKey(item.Name))
                        parameters.Add(Parameters[item.Name]);
                    else
                        parameters.Add(null);
                }
            }

            return func.Invoke(service, parameters.ToArray());
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
