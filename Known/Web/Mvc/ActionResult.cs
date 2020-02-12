namespace Known.Web.Mvc
{
    /// <summary>
    /// Action结果。
    /// </summary>
    public abstract class ActionResult
    {
        /// <summary>
        /// 空结果。
        /// </summary>
        public static ActionResult Empty = new EmptyResult();

        protected ActionResult() { }

        protected ActionResult(ControllerContext context)
        {
            Context = context;
        }

        protected ControllerContext Context { get; }

        public virtual void Execute()
        {
        }
    }

    class EmptyResult : ActionResult
    {
    }
}
