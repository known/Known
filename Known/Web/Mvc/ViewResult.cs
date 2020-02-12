using System.IO;

namespace Known.Web.Mvc
{
    public class ViewResult : ActionResult
    {
        private readonly object obj = new object();

        public ViewResult(ControllerContext context) : base(context) { }

        public override void Execute()
        {
            var html = "Hello World!";
            var asm = Context.Type.Assembly;
            var names = asm.GetManifestResourceNames();
            foreach (var item in names)
            {
                if (item.Contains($"{Context.ControllerName}.{Context.ActionName}"))
                {
                    lock (obj)
                    {
                        var stream = asm.GetManifestResourceStream(item);
                        if (stream != null)
                        {
                            using (var sr = new StreamReader(stream))
                            {
                                html = sr.ReadToEnd();
                            }
                        }
                    }
                    break;
                }
            }

            html = html.Replace("~/", "/");

            Context.HttpContext.Response.Write(html);
        }
    }
}
