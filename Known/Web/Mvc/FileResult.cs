namespace Known.Web.Mvc
{
    /// <summary>
    /// 文件结果类。
    /// </summary>
    public class FileResult : ActionResult
    {
        private readonly byte[] content;
        private readonly string fileName;
        private readonly string mimeType;

        /// <summary>
        /// 初始化一个文件结果类的实例。
        /// </summary>
        /// <param name="context">控制器上下文对象。</param>
        /// <param name="content">文件内容。</param>
        /// <param name="fileName">文件名。</param>
        /// <param name="mimeType">文件类型。</param>
        public FileResult(ControllerContext context, byte[] content, string fileName, string mimeType = null) : base(context)
        {
            this.content = content;
            this.fileName = fileName;
            this.mimeType = mimeType ?? MimeTypes.ApplicationOctetStream;
        }

        /// <summary>
        /// 执行Action操作。
        /// </summary>
        public override void Execute()
        {
            var response = Context.HttpContext.Response;
            response.AddHeader("Content-Disposition", $"filename={fileName}");
            response.ContentType = mimeType;
            response.BinaryWrite(content);
        }
    }
}
