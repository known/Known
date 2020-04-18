using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace Known.Web
{
    class ResViewEngine
    {
        private readonly static object obj = new object();
        private readonly static Dictionary<int, string> styles = new Dictionary<int, string>();
        private readonly static Dictionary<int, string> scripts = new Dictionary<int, string>();

        internal static string GetView(ControllerContext context)
        {
            return GetContent(context);
        }

        internal static string GetPartial(ControllerContext context, string name)
        {
            return GetContent(context, true, name);
        }

        internal static string GetStyle(int id)
        {
            if (!styles.ContainsKey(id))
                return string.Empty;

            return styles[id];
        }

        internal static void SetStyle(int id, string style)
        {
            styles[id] = style;
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

        private static string GetContent(ControllerContext context, bool isPartial = false, string partialName = null)
        {
            var text = string.Empty;
            var assembly = context.Controller.GetType().Assembly;

            lock (obj)
            {
                var controller = context.RouteData.Values["controller"];
                var action = isPartial
                           ? partialName.Replace("/", ".")
                           : context.RouteData.Values["action"];
                var name = $"{controller}.{action}";
                text = Utils.GetResource(assembly, name);
            }

            if (string.IsNullOrWhiteSpace(text))
                return "<h1>Hello World!</h1>";

            var layout = string.Empty;
            if (!isPartial && !text.Contains("<html>"))
                layout = Utils.GetResource(assembly, "Views.Layout");

            if (!isPartial)
            {
                var parser = new ViewParser(context.HttpContext, text, layout);
                lock (obj)
                {
                    parser.Parse();
                }

                text = parser.Html;
            }

            return ReplaceHtml(text);
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
        private readonly string layout;

        public ViewParser(HttpContextBase context, string text, string layout = null)
        {
            this.context = context;
            this.text = text;
            this.layout = layout;
        }

        public string Html { get; private set; }

        public void Parse()
        {
            var html = GetHtml(text);
            if (string.IsNullOrWhiteSpace(html))
                html = text;

            var script = GetScript(context, text);
            var style = GetStyle(context, text);

            if (!string.IsNullOrWhiteSpace(layout))
            {
                html = layout.Replace("<div id=\"app\"></div>", $"<div id=\"app\">{html}</div>");
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(style.Item2))
                    html = html.Replace(style.Item2, "")
                               .Replace($"<style></style>{Environment.NewLine}", ""); ;
                if (!string.IsNullOrWhiteSpace(script.Item2))
                    html = html.Replace(script.Item2, "")
                               .Replace($"<script></script>{Environment.NewLine}", "");
            }

            if (!string.IsNullOrWhiteSpace(style.Item1))
                html = html.Replace("</head>", $"{style.Item1}{Environment.NewLine}</head>");
            if (!string.IsNullOrWhiteSpace(script.Item1))
                html = html.Replace("</body>", $"{script.Item1}{Environment.NewLine}</body>");

            Html = html;
        }

        private static Tuple<string, string> GetStyle(HttpContextBase context, string text)
        {
            var style = SubString(text, "<style>", "</style>");
            if (string.IsNullOrWhiteSpace(style))
                return new Tuple<string, string>("", "");

            var id = context.Request.Path.GetHashCode();
            ResViewEngine.SetStyle(id, style.Replace("~/", "/"));
            var html = $"<link rel=\"stylesheet\" href=\"~/Home/Style?id={id}\" media=\"all\">";
            return new Tuple<string, string>(html, style);
        }

        private static string GetHtml(string text)
        {
            return SubString(text, "<template>", "</template>");
        }

        private static Tuple<string, string> GetScript(HttpContextBase context, string text)
        {
            var script = SubString(text, "<script>", "</script>");
            if (string.IsNullOrWhiteSpace(script))
                return new Tuple<string, string>("", "");

            var id = context.Request.Path.GetHashCode();
            ResViewEngine.SetScript(id, script.Replace("~/", "/"));
            var html = $"<script src=\"~/Home/Script?id={id}\"></script>";
            return new Tuple<string, string>(html, script);
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
