#if !NET6_0
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using Known.Core;

namespace Known.Web
{
    public interface IMinify
    {
        string Css(string source);
        string Js(string source);
        string Html(string source);
    }

    class ViewAppInfo
    {
        internal string AppId { get; set; }
        internal string AppName { get; set; }
        internal string AppLang { get; set; }
        internal string SupportName { get; set; }
        internal string SupportUrl { get; set; }

        internal static ViewAppInfo Create(string host, UserInfo user)
        {
            var app = Config.App;
            var info = new ViewAppInfo
            {
                AppId = app.AppId,
                AppName = app.AppName,
                SupportName = app.SupportName,
                SupportUrl = app.SupportUrl
            };

            if (user != null)
            {
                info.AppId = user.AppId;
                info.AppName = user.AppName;
                info.SupportName = user.SupportName;
                info.SupportUrl = user.SupportUrl;
            }

            return info;
        }
    }

    class ViewContext
    {
        internal ViewAppInfo App { get; set; }
        internal Assembly LayoutAssembly { get; set; }
        internal Assembly Assembly { get; set; }
        internal HttpContext HttpContext { get; set; }
        internal IMinify Minify { get; set; }
        internal string Url { get; set; }
        internal string Controller { get; set; }
        internal string Action { get; set; }
        internal string ParamId { get; set; }
        internal string PartialName { get; set; }

        internal bool IsPartial
        {
            get { return !string.IsNullOrEmpty(PartialName); }
        }
    }

    sealed class ViewEngine
    {
        private readonly static object obj = new object();
        private readonly static Dictionary<int, string> styles = new Dictionary<int, string>();
        private readonly static Dictionary<int, string> scripts = new Dictionary<int, string>();

        private ViewEngine() { }

        internal static string GetView(ViewContext context)
        {
            return GetContent(context);
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

        internal static bool ExistsView(string url, out string path)
        {
            var root = Config.RootPath;
            var urls = url.Split('/');
            path = string.Empty;
            if (urls.Length == 1)
            {
                path = Path.Combine(root, url + ".html");
                if (File.Exists(path))
                    return true;

                path = Path.Combine(root, "pages");
                path = Path.Combine(path, url + ".html");
                return File.Exists(path);
            }
            else if (urls.Length > 1)
            {
                path = Path.Combine(root, "pages");
                path = Path.Combine(path, url.Replace("/", @"\") + ".html");
                if (File.Exists(path))
                    return true;

                path = Path.Combine(root, "static");
                path = Path.Combine(path, urls[0]);
                path = Path.Combine(path, "page");
                path = Path.Combine(path, string.Join(@"\", urls.Skip(1).ToArray()) + ".html");
                if (File.Exists(path))
                    return true;
            }

            return false;
        }

        private static string GetContent(ViewContext context)
        {
            var assembly = context.Assembly;
            var text = string.Empty;

            lock (obj)
            {
                var name = context.Url;
                if (string.IsNullOrEmpty(name))
                {
                    var controller = context.Controller;
                    var action = context.IsPartial
                               ? context.PartialName.Replace("/", ".")
                               : context.Action;
                    name = $"{controller}.{action}";
                }
                text = GetViewContent(assembly, name);
            }

            if (string.IsNullOrEmpty(text))
                return "<h1>Hello World!</h1>";

            var layout = string.Empty;
            if (!context.IsPartial && !text.Contains("<html>"))
                layout = GetViewContent(context.LayoutAssembly, "Views.Layout");

            if (!context.IsPartial)
            {
                var parser = new ViewParser(context, text, layout);
                lock (obj)
                {
                    parser.Parse();
                }

                text = parser.Html;
            }

            var html = ReplaceHtml(text, context);

            if (context.Minify != null)
                html = context.Minify.Html(html);

            return html;
        }

        private static string GetViewContent(Assembly assembly, string name)
        {
            if (ExistsView(name.Replace('.', '/'), out string path))
                return File.ReadAllText(path);

            return Utils.GetResource(assembly, name);
        }

        internal static string ReplaceHtml(string html, ViewContext context)
        {
            var app = context.App;
            var path = context.HttpContext.Request.ApplicationPath.TrimEnd('/');
            var htmls = new List<string>();
            var lines = html.Split(Environment.NewLine.ToCharArray());
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (string.IsNullOrEmpty(line))
                    continue;

                if (line.Contains("@@"))
                    line = line.Replace("@@", "@");

                if (line.Contains("@year"))
                    line = line.Replace("@year", DateTime.Now.ToString("yyyy"));
                if (line.Contains("@Config.AppId"))
                    line = line.Replace("@Config.AppId", app.AppId.ToLower());
                if (line.Contains("@Config.AppName"))
                    line = line.Replace("@Config.AppName", app.AppName);
                if (line.Contains("@Config.AppLang"))
                    line = line.Replace("@Config.AppLang", app.AppLang);
                if (line.Contains("@Config.SupportName"))
                    line = line.Replace("@Config.SupportName", app.SupportName);
                if (line.Contains("@Config.SupportUrl"))
                    line = line.Replace("@Config.SupportUrl", app.SupportUrl);

                if (line.Contains("@Config.AppSys"))
                {
                    var a = context.HttpContext.Request.QueryString["a"];
                    var s = context.HttpContext.Request.QueryString["s"];
                    if (string.IsNullOrEmpty(a))
                        a = app.AppId.ToLower();
                    if (string.IsNullOrEmpty(s))
                        s = app.AppId.ToLower();
                    var appSys = a == s ? $"{a}/page" : $"{a}/{s}";
                    line = line.Replace("@Config.AppSys", appSys);
                }

                if (line.Contains("@time"))
                    line = ReplaceTimeVersion(context, line);
                if (line.Contains("~/"))
                    line = line.Replace("~/", path + "/");

                if (!string.IsNullOrEmpty(line))
                    htmls.Add(line);
            }
            return string.Join(Environment.NewLine, htmls.ToArray());
        }

        private static string ReplaceTimeVersion(ViewContext context, string html)
        {
            //href="~/static/css/login.min.css?v=@time"
            var bundles = BundleConfig.Load();
            if (bundles == null || bundles.Count == 0)
                return ReplaceTimeVersion(context.HttpContext, html);

            var url = html.Split('?')[0];
            url = url.Split('~')[1].TrimStart('/');
            var bundle = bundles.FirstOrDefault(b => b.OutputFileName == url);
            if (bundle == null)
                return ReplaceTimeVersion(context.HttpContext, html);

            var lines = new List<string>();
            foreach (var item in bundle.InputFiles)
            {
                if (item.Contains("*."))
                {
                    var path = Path.Combine(Config.RootPath, item.Split('*')[0]);
                    var dirs = Directory.GetDirectories(path);
                    foreach (var dir in dirs)
                    {
                        var dirFiles = Directory.GetFiles(dir);
                        foreach (var file in dirFiles)
                        {
                            AddDevStaticFile(lines, file);
                        }
                    }

                    var files = Directory.GetFiles(path);
                    foreach (var file in files)
                    {
                        AddDevStaticFile(lines, file);
                    }
                }
                else
                {
                    var file = Path.Combine(Config.RootPath, item);
                    AddDevStaticFile(lines, file);
                }
            }

            return string.Join(Environment.NewLine, lines.ToArray());
        }

        private static void AddDevStaticFile(List<string> lines, string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            var time = fileInfo.LastWriteTime.ToString("yyMMddHHmmss");
            var file = fileName.Replace(Config.RootPath, "").Replace("\\", "/");
            if (file.EndsWith(".css"))
            {
                var line = $"<link rel=\"stylesheet\" href=\"~/{file}?v={time}\">";
                if (!lines.Contains(line))
                {
                    lines.Add(line);
                }
            }
            else if (file.EndsWith(".js"))
            {
                var line = $"<script src=\"~/{file}?v={time}\"></script>";
                if (!lines.Contains(line))
                {
                    lines.Add(line);
                }
            }
        }

        private static string ReplaceTimeVersion(HttpContext context, string html)
        {
            var url = html.Split('?')[0];
            url = url.Split('~')[1];
            var fileName = context.Server.MapPath($"~{url}");
            var lastTime = File.GetLastWriteTime(fileName);
            var time = lastTime.ToString("yyMMddHHmmss");
            return html.Replace("@time", time);
        }
    }

    class ViewParser
    {
        private readonly ViewContext context;
        private readonly string text;
        private readonly string layout;
        private readonly DateTime lastTime;

        internal ViewParser(ViewContext context, string text, string layout = null)
        {
            this.context = context;
            this.text = text;
            this.layout = layout;
            this.lastTime = File.GetLastWriteTime(context.Assembly.Location);
        }

        internal string Html { get; private set; }

        internal void Parse()
        {
            var id = context.HttpContext.Request.Path.GetHashCode();
            var time = lastTime.ToString("yyMMddHHmmss");
            var script = GetScript(context, id, time, text);
            var style = GetStyle(context, id, time, text);

            var html = text;
            if (!string.IsNullOrEmpty(style.Content))
                html = html.Replace(style.Content, "")
                           .Replace($"<style></style>", "");
            if (!string.IsNullOrEmpty(script.Content))
                html = html.Replace(script.Content, "")
                           .Replace($"<script></script>", "");

            if (!string.IsNullOrEmpty(layout))
                html = layout.Replace("<div id=\"app\"></div>", html);

            if (!string.IsNullOrEmpty(style.Html))
                html = html.Replace("</head>", $"    {style.Html}{Environment.NewLine}</head>");
            if (!string.IsNullOrEmpty(script.Html))
                html = html.Replace("</body>", $"    {script.Html}{Environment.NewLine}</body>");

            Html = html;
        }

        class HtmlInfo
        {
            internal HtmlInfo(string html, string content)
            {
                Html = html;
                Content = content;
            }

            internal string Html { get; }
            internal string Content { get; }
        }

        private static HtmlInfo GetStyle(ViewContext context, int id, string time, string text)
        {
            var style = SubString(text, "<style>", "</style>");
            if (string.IsNullOrEmpty(style))
                return new HtmlInfo("", "");

            var content = ViewEngine.ReplaceHtml(style, context);
            if (context.Minify != null)
                content = context.Minify.Css(content);
            ViewEngine.SetStyle(id, content);

            var html = $"<link rel=\"stylesheet\" href=\"~/Home/Style?id={id}&v={time}\" media=\"all\">";
            return new HtmlInfo(html, style);
        }

        private static HtmlInfo GetScript(ViewContext context, int id, string time, string text)
        {
            var script = SubString(text, "<script>", "</script>");
            if (string.IsNullOrEmpty(script))
                return new HtmlInfo("", "");

            var content = ViewEngine.ReplaceHtml(script, context);
            if (context.Minify != null)
                content = context.Minify.Js(content);
            ViewEngine.SetScript(id, content);

            var html = $"<script src=\"~/Home/Script?id={id}&v={time}\"></script>";
            return new HtmlInfo(html, script);
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
#endif