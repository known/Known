using System;
using Known.Data;
using Known.Mapping;

namespace Known
{
    /// <summary>
    /// 业务服务基类。
    /// </summary>
    public abstract class ServiceBase
    {
        /// <summary>
        /// 创建一个业务服务类实例。
        /// </summary>
        /// <param name="context">上下文对象。</param>
        protected ServiceBase(Context context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// 取得上下文对象。
        /// </summary>
        public Context Context { get; }

        /// <summary>
        /// 加载指定类型的业务服务对象。
        /// </summary>
        /// <typeparam name="T">业务服务类型。</typeparam>
        /// <returns>业务服务对象。</returns>
        protected T LoadService<T>() where T : ServiceBase
        {
            return Container.Resolve<T>();
        }

        /// <summary>
        /// 加载指定类型的数据仓库对象。
        /// </summary>
        /// <typeparam name="T">数据仓库类型。</typeparam>
        /// <returns>数据仓库对象。</returns>
        protected T LoadRepository<T>() where T : IRepository
        {
            return ObjectFactory.CreateRepository<T>(Context);
        }
    }

    /// <summary>
    /// 指定数据仓库类型的业务服务基类。
    /// </summary>
    /// <typeparam name="T">数据仓库类型。</typeparam>
    public abstract class ServiceBase<T> : ServiceBase
        where T : IRepository
    {
        /// <summary>
        /// 创建一个业务服务类实例。
        /// </summary>
        /// <param name="context">上下文对象。</param>
        protected ServiceBase(Context context) : base(context)
        {
        }

        /// <summary>
        /// 取得数据仓库对象。
        /// </summary>
        protected T Repository
        {
            get { return LoadRepository<T>(); }
        }

        /// <summary>
        /// 根据主键 ID 获取实体对象。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="id">实体ID。</param>
        /// <param name="defaultEntity">为空时的默认对象。</param>
        /// <returns>实体对象。</returns>
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
