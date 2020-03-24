namespace Known.Web.Mvc
{
    /// <summary>
    /// Action结果抽象类。
    /// </summary>
    public abstract class ActionResult
    {
        /// <summary>
        /// 空结果。
        /// </summary>
        public static ActionResult Empty = new EmptyResult();

        /// <summary>
        /// 初始化一个Action结果类的实例。
        /// </summary>
        protected ActionResult() { }

        /// <summary>
        /// 初始化一个Action结果类的实例。
        /// </summary>
        /// <param name="context">控制器上下文对象。</param>
        protected ActionResult(ControllerContext context)
        {
            Context = context;
        }

        /// <summary>
        /// 取得控制器上下文对象。
        /// </summary>
        public ControllerContext Context { get; }

        /// <summary>
        /// 执行Action操作。
        /// </summary>
        public virtual void Execute()
        {
        }
    }

    class EmptyResult : ActionResult
    {
    }
}
