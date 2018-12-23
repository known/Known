using System;
using Known.Data;

namespace Known
{
    public abstract class ServiceBase
    {
        public ServiceBase(Context context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Context Context { get; }

        protected T LoadService<T>() where T : ServiceBase
        {
            return Container.Load<T>(typeof(T).Name);
        }

        protected T LoadRepository<T>() where T : IRepository
        {
            return ObjectFactory.CreateRepository<T>(Context);
        }
    }

    public abstract class ServiceBase<TRepository> : ServiceBase
        where TRepository : IRepository
    {
        public ServiceBase(Context context) : base(context)
        {
        }

        protected TRepository Repository
        {
            get { return LoadRepository<TRepository>(); }
        }
    }
}
