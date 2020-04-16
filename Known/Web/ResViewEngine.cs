using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Known.Web
{
    class ResViewEngine
    {
        private readonly static object obj = new object();
        private readonly static Dictionary<int, string> scripts = new Dictionary<int, string>();

        internal static string GetView(ControllerContext context)
        {
            return GetContent(context);
        }

        internal static string GetPartial(ControllerContext context)
        {
            return GetContent(context, true);
        }

        internal static string GetScript(int id)
        {
            if (!scripts.ContainsKey(id))
                return string.Empty;

            return scripts[id];
        }

        internal static void SetScript(int id, string script)
        {
            scripts[id] = script;
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
                return "<h1>Hello World!</h1>";

            if (text.Contains("<html>"))
                return ReplaceHtml(text);

            var layout = string.Empty;
            if (!isPartial)
                layout = Utils.GetResource(assembly, "Views.Layout");

            var parser = new ViewParser(context.HttpContext, text, layout);
            lock (obj)
            {
                parser.Parse();
            }

            return ReplaceHtml(parser.Html);
        }

        private static string ReplaceHtml(string html)
        {
            return html.Replace("~/", "/").Replace("@AppName", Config.AppName);
        }
    }

    class ViewParser
    {
        private readonly HttpContextBase context;
        private readonly string text;
        private string layout;

        public ViewParser(HttpContextBase context, string text, string layout = null)
        {
            this.context = context;
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

            var script = string.Empty;
            var style = GetStyle(text);
            var js = GetScript(text);
            if (!string.IsNullOrWhiteSpace(js))
            {
                var id = context.Request.Path.GetHashCode();
                ResViewEngine.SetScript(id, js.Replace("~/", "/"));
                script = $"<script src=\"~/Home/Script?id={id}\"></script>";
            }

            if (!string.IsNullOrWhiteSpace(layout))
            {
                if (!string.IsNullOrWhiteSpace(style))
                    layout = layout.Replace("</head>", $"<style>{style}</style></head>");
                if (!string.IsNullOrWhiteSpace(script))
                    layout = layout.Replace("</body>", $"{script}</body>");
                layout = layout.Replace("<div id=\"app\"></div>", $"<div id=\"app\">{html}</div>");
                sb.Append(layout);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(style))
                    sb.Append($"<style>{style}</style>");
                sb.Append(html);
                if (!string.IsNullOrWhiteSpace(script))
                    sb.Append(script);
            }

            Html = sb.ToString();
        }

        private static string GetStyle(string text)
        {
            return SubString(text, "<style>", "</style>");
        }

        private static string GetHtml(string text)
        {
            return SubString(text, "<template>", "</template>");
        }

        private static string GetScript(string text)
        {
            return SubString(text, "<script>", "</script>");
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
