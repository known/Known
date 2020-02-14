using System.IO;
using System.Linq;
using System.Reflection;

namespace Known.Web.Mvc
{
    /// <summary>
    /// 视图结果类。
    /// </summary>
    public class ViewResult : ActionResult
    {
        private readonly static object obj = new object();
        private readonly bool isPartial;

        /// <summary>
        /// 初始化一个视图结果类的实例。
        /// </summary>
        /// <param name="context">控制器上下文对象。</param>
        /// <param name="isPartial">是否是部分视图。</param>
        public ViewResult(ControllerContext context, bool isPartial = false) : base(context)
        {
            this.isPartial = isPartial;
        }

        /// <summary>
        /// 执行Action操作。
        /// </summary>
        public override void Execute()
        {
            var text = "Hello World!";
            var assembly = Context.Route.Controller.Assembly;
            var names = assembly.GetManifestResourceNames();
            foreach (var item in names)
            {
                if (item.Contains($"{Context.ControllerName}.{Context.ActionName}."))
                {
                    text = GetContent(assembly, item);
                    break;
                }
            }

            var layout = string.Empty;
            if (!isPartial && !text.Contains("<html>"))
            {
                var name = names.FirstOrDefault(n => n.Contains("Views.Layout"));
                layout = GetContent(assembly, name);
            }

            var parser = new ViewParser(text, layout);
            lock (obj)
            {
                parser.Parse();
            }

            Context.HttpContext.Response.Write(parser.Html);
        }

        private static string GetContent(Assembly assembly, string name)
        {
            var html = string.Empty;
            if (string.IsNullOrWhiteSpace(name))
                return html;

            lock (obj)
            {
                var stream = assembly.GetManifestResourceStream(name);
                if (stream != null)
                {
                    using (var sr = new StreamReader(stream))
                    {
                        html = sr.ReadToEnd();
                    }
                }
            }

            return html;
        }
    }
}
