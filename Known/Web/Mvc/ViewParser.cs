using System.Text;
using Known.Extensions;

namespace Known.Web.Mvc
{
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
