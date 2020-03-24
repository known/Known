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
            var assembly = Context.Action.Controller.Assembly;
            var text = GetContent(assembly);
            if (string.IsNullOrWhiteSpace(text))
            {
                Context.HttpContext.Response.Write("Hello World!");
                return;
            }

            var layout = string.Empty;
            if (!isPartial && !text.Contains("<html>"))
                layout = GetResource(assembly, "Views.Layout");

            var parser = new ViewParser(text, layout);
            lock (obj)
            {
                parser.Parse();
            }

            Context.HttpContext.Response.Write(parser.Html);
        }

        /// <summary>
        /// 获取View页面内容。
        /// </summary>
        /// <param name="assembly">程序集。</param>
        /// <returns>View页面内容。</returns>
        protected virtual string GetContent(Assembly assembly)
        {
            var name = $"{Context.ControllerName}.{Context.ActionName}";
            return GetResource(assembly, name);
        }

        /// <summary>
        /// 获取程序集资源文件内容。
        /// </summary>
        /// <param name="assembly">程序集。</param>
        /// <param name="name">资源名称。</param>
        /// <returns>资源文件内容。</returns>
        protected static string GetResource(Assembly assembly, string name)
        {
            var text = string.Empty;
            if (string.IsNullOrWhiteSpace(name))
                return text;

            var names = assembly.GetManifestResourceNames();
            name = names.FirstOrDefault(n => n.Contains(name));
            if (string.IsNullOrWhiteSpace(name))
                return text;

            lock (obj)
            {
                var stream = assembly.GetManifestResourceStream(name);
                if (stream != null)
                {
                    using (var sr = new StreamReader(stream))
                    {
                        text = sr.ReadToEnd();
                    }
                }
            }

            return text;
        }
    }
}
