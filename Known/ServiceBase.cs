using System;
using Known.Data;
using Known.Mapping;

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

        protected TEntity GetEntityById<TEntity>(string id, TEntity defaultEntity)
            where TEntity : EntityBase
        {
            var entity = Repository.QueryById<TEntity>(id);
            if (entity == null)
            {
                entity = defaultEntity;
            }
            return entity;
        }
    }
}
