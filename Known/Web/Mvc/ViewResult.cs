namespace Known.Web.Mvc
{
    public class ViewResult : ActionResult
    {
        public ViewResult(ControllerContext context) : base(context) { }

        public override void Execute()
        {
            Context.HttpContext.Response.Write(Context.Action.Name);
        }
    }
}
