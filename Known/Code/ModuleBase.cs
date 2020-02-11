using System;

namespace Known
{
    /// <summary>
    /// 模块抽象基类。
    /// </summary>
    public abstract class ModuleBase
    {
        /// <summary>
        /// 取得模块名称。
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 取得模块图标。
        /// </summary>
        public abstract string Icon { get; }

        /// <summary>
        /// 取得模块类型。
        /// </summary>
        public Type Type
        {
            get { return GetType(); }
        }

        /// <summary>
        /// 初始化模块。
        /// </summary>
        /// <param name="app">APP信息。</param>
        /// <param name="context">系统上下文对象。</param>
        public virtual void Init(AppInfo app, Context context) { }

        /// <summary>
        /// 获取模块页面视图对象。
        /// </summary>
        /// <param name="order">页面序号。</param>
        /// <param name="code">页面代码。</param>
        /// <param name="name">页面名称。</param>
        /// <param name="icon">页面图标。</param>
        /// <returns>模块页面视图对象。</returns>
        protected PageView Page(int order, string code, string name, string icon)
        {
            return new PageView(Type, order, code, name, icon);
        }
    }
}
