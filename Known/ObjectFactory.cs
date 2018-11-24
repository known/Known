using System;
using System.Collections;
using Known.Data;

namespace Known
{
    public class ObjectFactory
    {
        private static Hashtable cached = new Hashtable();

        public static T CreateService<T>(Context context) where T : ServiceBase
        {
            var type = typeof(T);
            if (!cached.ContainsKey(type))
            {
                lock (cached.SyncRoot)
                {
                    if (!cached.ContainsKey(type))
                    {
                        cached[type] = Activator.CreateInstance(type, context);
                    }
                }
            }

            return (T)cached[type];
        }

        public static T CreateRepository<T>(Context context) where T : IRepository
        {
            var repository = Container.Load<T>();
            if (repository != null)
                return repository;

            var type = typeof(T);
            if (!cached.ContainsKey(type))
            {
                lock (cached.SyncRoot)
                {
                    if (!cached.ContainsKey(type))
                    {
                        var typeName = type.FullName.Replace(".I", ".");
                        var objType = type.Assembly.GetType(typeName);
                        if (objType != null)
                        {
                            cached[type] = Activator.CreateInstance(objType, context.Database);
                        }
                    }
                }
            }

            return (T)cached[type];
        }
    }
}
