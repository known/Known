using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Known.Extensions;

namespace Known.Web.Mvc
{
    public class ViewResult : ActionResult
    {
        private readonly static object obj = new object();

        public ViewResult(ControllerContext context) : base(context) { }

        public override void Execute()
        {
            var text = "Hello World!";
            var assembly = Context.Type.Assembly;
            var names = assembly.GetManifestResourceNames();
            foreach (var item in names)
            {
                if (item.Contains($"{Context.ControllerName}.{Context.ActionName}"))
                {
                    text = GetContent(assembly, item);
                    break;
                }
            }

            var layout = string.Empty;
            if (!text.Contains("<html>"))
            {
                var name = names.FirstOrDefault(n => n.Contains("Views.Layout"));
                layout = GetContent(assembly, name);
            }

            var parser = new HtmlParser(text, layout);
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

    class HtmlParser
    {
        private string text;
        private string layout;

        public HtmlParser(string text, string layout = null)
        {
            this.text = text;
            this.layout = layout;
        }

        public string Html { get; private set; }

        public void Parse()
        {
            var sb = new StringBuilder();

            var html = text.SubString("<template>", "</template>");
            if (string.IsNullOrWhiteSpace(html))
                html = text;

            var style = text.SubString("<template id=\"style\">", "</template>");
            var script = text.SubString("<template id=\"script\">", "</template>");

            if (!string.IsNullOrWhiteSpace(layout))
            {
                if (!string.IsNullOrWhiteSpace(style))
                    layout = layout.Replace("</head>", style + "</head>");
                if (!string.IsNullOrWhiteSpace(script))
                    layout = layout.Replace("</body>", script + "</body>");
                layout = layout.Replace("<div id=\"app\"></div>", $"<div id=\"app\">{html}</div>");
                sb.Append(layout);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(style))
                    sb.Append(style);
                sb.Append(html);
                if (!string.IsNullOrWhiteSpace(script))
                    sb.Append(html);
            }

            Html = sb.ToString();
            Html = Html.Replace("~/", "/");
        }
    }
}
