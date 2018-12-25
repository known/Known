using System;

namespace Known
{
    public sealed class ServiceUtils
    {
        public static object Execute(string userName, string module, string method, object[] parameters)
        {
            var service = GetService(userName, module);
            if (service == null)
                throw new Exception($"暂未实现{module}模块！");

            var func = service.GetType().GetMethod(method);
            if (func == null)
                throw new Exception($"{module}模块暂未实现{method}方法！");

            return func.Invoke(service, parameters);
        }

        private static object GetService(string userName, string module)
        {
            var service = Container.Load($"{module}Service");
            if (service != null)
            {
                ((ServiceBase)service).Context.UserName = userName;
            }
            return service;
        }
    }
}
