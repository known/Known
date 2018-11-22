using System;
using Known.Data;
using Known.Log;

namespace Known
{
    public abstract class ServiceBase
    {
        public ServiceBase(Context context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Context Context { get; }

        public ILogger Logger
        {
            get { return Context.Logger; }
        }

        protected T LoadBusiness<T>() where T : ServiceBase
        {
            return ObjectFactory.CreateService<T>(Context);
        }

        protected T LoadRepository<T>() where T : IRepository
        {
            return ObjectFactory.CreateRepository<T>(Context);
        }
    }
}
