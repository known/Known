namespace Known.Web.Mvc
{
    /// <summary>
    /// 内容结果。
    /// </summary>
    public class ContentResult : ActionResult
    {
        public ContentResult(ControllerContext context, string content, string mimeType = null) : base(context)
        {
            Content = content;
            MimeType = mimeType ?? MimeTypes.TextPlain;
        }

        public string Content { get; }
        public string MimeType { get; }

        public override void Execute()
        {
            Context.HttpContext.Response.ContentType = MimeType;
            Context.HttpContext.Response.Write(Content);
        }
    }
}
