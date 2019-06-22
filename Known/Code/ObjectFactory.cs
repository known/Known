using System;
using Known.Data;

namespace Known
{
    /// <summary>
    /// 应用程序对象工厂类。
    /// </summary>
    public sealed class ObjectFactory
    {
        /// <summary>
        /// 创建一个继承 ServiceBase 的泛型服务对象。
        /// </summary>
        /// <typeparam name="T">服务对象类型。</typeparam>
        /// <param name="context">上下文对象，用于构造函数参数。</param>
        /// <returns>泛型服务对象。</returns>
        public static T CreateService<T>(Context context) where T : ServiceBase
        {
            var service = Container.Resolve<T>();
            if (service == null)
            {
                var type = typeof(T);
                var instance = Activator.CreateInstance(type, context);
                Container.Register<T>(instance);
            }

            return Container.Resolve<T>();
        }

        /// <summary>
        /// 创建一个继承 IRepository 的泛型数据仓库对象。
        /// </summary>
        /// <typeparam name="T">数据仓库类型。</typeparam>
        /// <param name="context">上下文对象，用于构造函数参数。</param>
        /// <returns>泛型数据仓库对象。</returns>
        public static T CreateRepository<T>(Context context) where T : IRepository
        {
            var repository = Container.Resolve<T>();
            if (repository == null)
            {
                var type = typeof(T);
                var typeName = type.FullName.Replace(".I", ".");
                var objType = type.Assembly.GetType(typeName);
                if (objType != null)
                {
                    context.Database.UserName = context.UserName;
                    var instance = Activator.CreateInstance(objType, context.Database);
                    Container.Register<T>(instance);
                }
            }

            return Container.Resolve<T>();
        }

        /// <summary>
        /// 创建一个泛型类型对象，如果 T 是接口，默认创建接口的默认实现对象。
        /// 例如：T 为 ITestService 接口，则默认创建 TestService 类实例。
        /// </summary>
        /// <typeparam name="T">泛型类型。</typeparam>
        /// <returns>泛型类型对象。</returns>
        public static T Create<T>()
        {
            var repository = Container.Resolve<T>();
            if (repository == null)
            {
                var type = typeof(T);
                var typeName = type.FullName.Replace(".I", ".");
                var objType = type.Assembly.GetType(typeName);
                if (objType != null)
                {
                    var instance = Activator.CreateInstance(objType);
                    Container.Register<T>(instance);
                }
            }

            return Container.Resolve<T>();
        }
    }
}
