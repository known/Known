using System.Text;
using System.Web.Mvc;

namespace Known.Web
{
    class ResViewEngine
    {
        private readonly static object obj = new object();

        internal static string GetView(ControllerContext context)
        {
            return GetContent(context);
        }

        internal static string GetPartial(ControllerContext context)
        {
            return GetContent(context, true);
        }

        private static string GetContent(ControllerContext context, bool isPartial = false)
        {
            var text = string.Empty;
            var assembly = context.Controller.GetType().Assembly;

            lock (obj)
            {
                var controller = context.RouteData.Values["controller"];
                var action = context.RouteData.Values["action"];
                var name = $"{controller}.{action}";
                text = Utils.GetResource(assembly, name);
            }

            if (string.IsNullOrWhiteSpace(text))
                return "Hello World!";

            var layout = string.Empty;
            if (!isPartial && !text.Contains("<html>"))
                layout = Utils.GetResource(assembly, "Views.Layout");

            var parser = new ViewParser(text, layout);
            lock (obj)
            {
                parser.Parse();
            }

            return parser.Html
                .Replace("@AppName", Config.AppName)
                .Replace("@Request.Path", context.RequestContext.HttpContext.Request.Path);
        }
    }

    class ViewParser
    {
        private readonly string text;
        private string layout;

        public ViewParser(string text, string layout = null)
        {
            this.text = text;
            this.layout = layout;
        }

        public string Html { get; private set; }

        public void Parse()
        {
            var sb = new StringBuilder();

            var html = GetHtml(text);
            if (string.IsNullOrWhiteSpace(html))
                html = text;

            var style = GetStyle(text);
            var script = GetScript(text);

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

        private static string GetStyle(string text)
        {
            return SubString(text, "<template id=\"style\">", "</template>");
        }

        private static string GetHtml(string text)
        {
            return SubString(text, "<template>", "</template>");
        }

        private static string GetScript(string text)
        {
            return SubString(text, "<template id=\"script\">", "</template>");
        }

        private static string SubString(string text, string start, string end)
        {
            var startIndex = text.IndexOf(start);
            if (startIndex < 0)
                return string.Empty;

            var endIndex = text.IndexOf(end, startIndex);
            return text.Substring(startIndex, endIndex - startIndex).Replace(start, "");
        }
    }
}
