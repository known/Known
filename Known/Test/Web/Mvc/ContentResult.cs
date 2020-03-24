namespace Known.Web.Mvc
{
    /// <summary>
    /// 内容结果类。
    /// </summary>
    public class ContentResult : ActionResult
    {
        private readonly string mimeType;
        private readonly string content;

        /// <summary>
        /// 初始化一个内容结果类的实例。
        /// </summary>
        /// <param name="context">控制器上下文对象。</param>
        /// <param name="content">内容字符串。</param>
        /// <param name="mimeType">MIME类型。</param>
        public ContentResult(ControllerContext context, string content, string mimeType = null) : base(context)
        {
            this.content = content;
            this.mimeType = mimeType ?? MimeTypes.TextPlain;
        }

        /// <summary>
        /// 执行Action操作。
        /// </summary>
        public override void Execute()
        {
            Context.HttpContext.Response.ContentType = mimeType;
            Context.HttpContext.Response.Write(content);
        }
    }
}
