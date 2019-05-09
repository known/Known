using System;
using Known.Data;

namespace Known
{
    public sealed class ObjectFactory
    {
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

        public static T CreateRepository<T>()
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
