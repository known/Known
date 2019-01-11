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
            return Container.Resolve<T>();
        }

        protected T LoadRepository<T>() where T : IRepository
        {
            return ObjectFactory.CreateRepository<T>(Context);
        }
    }

    public abstract class ServiceBase<T> : ServiceBase
        where T : IRepository
    {
        public ServiceBase(Context context) : base(context)
        {
        }

        protected T Repository
        {
            get { return LoadRepository<T>(); }
        }
    }
}
