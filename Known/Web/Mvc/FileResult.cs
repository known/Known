namespace Known.Web.Mvc
{
    /// <summary>
    /// 文件结果。
    /// </summary>
    public class FileResult : ActionResult
    {
        public FileResult(ControllerContext context) : base(context) { }
    }
}
