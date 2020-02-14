namespace Known
{
    /// <summary>
    /// 模块初始化者抽象类。
    /// </summary>
    public abstract class Initializer
    {
        /// <summary>
        /// 初始化模块。
        /// </summary>
        /// <param name="context">程序上下文对象。</param>
        public abstract void Initialize(Context context);
    }
}
