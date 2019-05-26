using System;
using System.Collections.Generic;

namespace Known
{
    /// <summary>
    /// 业务服务执行者类。
    /// </summary>
    public class ServiceExecuter
    {
        /// <summary>
        /// 创建一个业务服务执行者实例。
        /// </summary>
        /// <param name="userName">当前用户名。</param>
        /// <param name="module">业务模块名。</param>
        public ServiceExecuter(string userName, string module)
            : this(userName, module, null)
        {
        }

        /// <summary>
        /// 创建一个业务服务执行者实例，指定执行方法。
        /// </summary>
        /// <param name="userName">当前用户名。</param>
        /// <param name="module">业务模块名。</param>
        /// <param name="method">业务方法名。</param>
        public ServiceExecuter(string userName, string module, string method)
        {
            UserName = userName;
            Module = module;
            Method = method;
        }

        /// <summary>
        /// 取得当前用户名。
        /// </summary>
        public string UserName { get; }

        /// <summary>
        /// 取得业务服务模块名。
        /// </summary>
        public string Module { get; }

        /// <summary>
        /// 取得或设置业务模块执行的方法名。
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// 执行单个参数的业务模块方法。
        /// </summary>
        /// <param name="parameter">方法参数。</param>
        /// <returns>业务模块方法的执行结果。</returns>
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

        /// <summary>
        /// 执行指定参数名称的业务模块方法。
        /// </summary>
        /// <param name="parameters">方法参数字典。</param>
        /// <returns>业务模块方法的执行结果。</returns>
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
