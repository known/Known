using System;
using System.Collections.Generic;
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
        /// 初始化一个业务服务类实例。
        /// </summary>
        /// <param name="context">上下文对象。</param>
        protected ServiceBase(Context context)
        {
            Context = context;
        }

        /// <summary>
        /// 取得上下文对象。
        /// </summary>
        public Context Context { get; private set; }

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

        internal void SetContext(Context context)
        {
            Context = context;
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
        /// 初始化一个业务服务类实例。
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

        /// <summary>
        /// 校验要操作的实体列表，返回错误信息。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="ids">实体ID数组。</param>
        /// <param name="entities">返回的实体对象列表。</param>
        /// <param name="checkAction">实体检查操作。</param>
        /// <returns>错误信息。</returns>
        protected string CheckEntities<TEntity>(string[] ids, out List<TEntity> entities, Action<TEntity, List<string>> checkAction = null)
            where TEntity : EntityBase
        {
            entities = Repository.QueryListById<TEntity>(ids);
            if (entities == null || entities.Count == 0)
                return "请至少选择一条记录进行操作！";

            var message = string.Empty;
            if (checkAction != null)
            {
                var errors = new List<string>();
                foreach (var item in entities)
                    checkAction(item, errors);

                if (errors.Count > 0)
                    message = string.Join(Environment.NewLine, errors);
            }

            return message;
        }
    }
}
