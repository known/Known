using System;

namespace Known
{
    /// <summary>
    /// 业务逻辑基类。
    /// </summary>
    public class BusinessBase
    {
        /// <summary>
        /// 构造函数，创建一个业务逻辑对象。
        /// </summary>
        /// <param name="context">上下文对象。</param>
        public BusinessBase(Context context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// 取得下上文对象。
        /// </summary>
        public Context Context { get; }

        /// <summary>
        /// 加载业务逻辑实例。
        /// </summary>
        /// <typeparam name="T">业务逻辑对象类型。</typeparam>
        /// <returns>业务逻辑实例。</returns>
        protected T LoadBusiness<T>() where T : BusinessBase
        {
            return BusinessFactory.Create<T>(Context);
        }
    }
}
