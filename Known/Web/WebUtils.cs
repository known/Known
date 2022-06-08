/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * ------------------------------------------------------------------------------- */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Known.Core;
#if NET6_0
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
#else
using System.Web;
#endif

namespace Known.Web
{
    sealed class WebUtils
    {
        private static readonly List<BundleConfig> bundles;
        private static readonly string rootPath = Config.ContentRootPath;
        private static string wwwrootName;

        static WebUtils()
        {
            bundles = BundleConfig.Load();
        }

        internal static bool CheckMobile(HttpRequest request)
        {
#if NET6_0
            var agent = request.Headers[HeaderNames.UserAgent].ToString();
#else
            var agent = request.UserAgent;
#endif
            if (agent.Contains("Windows NT") || agent.Contains("Macintosh"))
                return false;

            bool flag = false;
            string[] keywords = { "Android", "iPhone", "iPod", "iPad", "Windows Phone", "MQQBrowser" };

            foreach (string item in keywords)
            {
                if (agent.Contains(item))
                {
                    flag = true;
                    break;
                }
            }

            return flag;
        }

        internal static string GetIndexHtml(HttpRequest request, string wwwroot = "")
        {
            wwwrootName = wwwroot;
#if NET6_0
            var token = request.Query["token"];
            var a = request.Query["a"];
            var m = request.Query["m"];
            var isHttps = request.Scheme == "https";
#else
            var token = request.QueryString["token"];
            var a = request.QueryString["a"];
            var m = request.QueryString["m"];
            var isHttps = request.Url.Scheme == "https";
#endif

            var app = Config.App;
            if (!string.IsNullOrEmpty(token))
                return GetLoginByToken(app.AppLang, token);

            var showCaptcha = app.ShowCaptcha.ToString().ToLower();
            var hasMobile = app.HasMobile.ToString().ToLower();
            var isTraditionView = app.TraditionView.ToString().ToLower();

            var user = UserHelper.GetUser(out _);
            app = Config.GetCurrentApp(user, a);
            var appId = app.AppId;
            var appName = app.AppName;
            var cssFiles = new List<string>();
            var jsFiles = new List<string>();
            var isApp = CheckMobile(request) || app.IsMobile;
            var isPage = !string.IsNullOrEmpty(m);
            InitStaticFiles(isApp, isPage, appId, app.AppLang, cssFiles, jsFiles);

            var lang = Thread.CurrentThread.CurrentUICulture.Name;
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine($"<html lang=\"{lang}\">");
            sb.AppendLine("<head>");
            sb.AppendLine("    <meta charset=\"utf-8\">");
            sb.AppendLine("    <meta name=\"renderer\" content=\"webkit\">");
            sb.AppendLine("    <meta name=\"viewport\" content=\"width=device-width,initial-scale=1\">");
            sb.AppendLine($"    <title>{appName}</title>");
            sb.AppendLine("    <base href=\"/\">");

            if (!isPage && isHttps)
            {
                sb.AppendLine("    <link rel=\"manifest\" href=\"manifest.json\">");
                sb.AppendLine("    <link rel=\"apple-touch-icon\" href=\"icon-512.png\" sizes=\"512x512\">");
                sb.AppendLine("    <link rel=\"apple-touch-icon\" href=\"icon-192.png\" sizes=\"192x192\">");
            }

            sb.AppendLine($"    <link rel=\"stylesheet\" href=\"{GetStaticUrl("/css/font-awesome.css")}\">");

            if (isPage)
            {
                sb.AppendLine($"    <link rel=\"stylesheet\" href=\"{GetStaticUrl("/libs/datepicker/datepicker.css")}\">");
            }

            foreach (var item in cssFiles)
            {
                sb.AppendLine($"    <link rel=\"stylesheet\" href=\"{item}\">");
            }

            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("    <div id=\"app\"></div>");
            sb.AppendLine($"    <script>var appId='{appId}',appName='{appName}',baseUrl='',baseApiUrl='',staticUrl='{wwwroot}',showCaptcha={showCaptcha},hasMobile={hasMobile},isTraditionView={isTraditionView};</script>");
            sb.AppendLine($"    <script src=\"{GetStaticUrl("/js/jquery.min.js")}\"></script>");

            if (isPage)
            {
                sb.AppendLine($"    <script src=\"{GetStaticUrl("/libs/datepicker/datepicker.js")}\"></script>");
                sb.AppendLine($"    <script src=\"{GetStaticUrl("/libs/datepicker/datepicker.zh-CN.js")}\"></script>");
            }

            foreach (var item in jsFiles)
            {
                sb.AppendLine($"    <script src=\"{item}\"></script>");
            }

            if (isPage)
            {
                if (user == null)
                    sb.AppendLine("    <script>top.app.login();</script>");
                else
                    sb.AppendLine("    <script>app.render();</script>");
            }
            else
            {
                if (!Config.IsInstalled)
                    sb.AppendLine("    <script>app.install();</script>");
                else if (!isApp)
                    sb.AppendLine("    <script>app.home();</script>");

                if (isHttps)
                    sb.AppendLine("    <script>registerWorker();</script>");
            }

            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            return sb.ToString();
        }

        private static string GetLoginByToken(string appLang, string token)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine($"<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("    <meta charset=\"utf-8\">");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("    <div id=\"app\"></div>");
            sb.AppendLine($"    <script src=\"{GetStaticUrl("/js/jquery.min.js")}\"></script>");
            sb.AppendLine($"    <script src=\"{GetStaticUrl($"/js/lang/{appLang}.min.js")}\"></script>");
            sb.AppendLine("    <script>var baseUrl='';</script>");
            sb.AppendLine($"    <script src=\"{GetStaticUrl("/js/index.min.js")}\"></script>");
            sb.AppendLine($"    <script>loginByToken('{token}');</script>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            return sb.ToString();
        }

        private static void InitStaticFiles(bool isApp, bool isPage, string appId, string appLang, List<string> cssFiles, List<string> jsFiles)
        {
            if (isApp)
                InitAppFiles(appId, appLang, cssFiles, jsFiles);
            else if (isPage)
                InitPageFiles(appId, appLang, cssFiles, jsFiles);
            else
                InitAdminFiles(appId, appLang, cssFiles, jsFiles);
        }

        private static void InitAppFiles(string appId, string appLang, List<string> cssFiles, List<string> jsFiles)
        {
            cssFiles.AddRange(GetStaticFileUrls("/css/app.min.css", true));
            jsFiles.AddRange(GetStaticFileUrls($"/js/lang/{appLang}.min.js", true));
            jsFiles.AddRange(GetStaticFileUrls("/js/app.min.js", true));

            if (appId != SystemService.DevId)
            {
                cssFiles.AddRange(GetStaticFileUrls($"/{appId}/app.min.css"));
                jsFiles.AddRange(GetStaticFileUrls($"/{appId}/app.min.js"));
            }
        }

        private static void InitAdminFiles(string appId, string appLang, List<string> cssFiles, List<string> jsFiles)
        {
            cssFiles.AddRange(GetStaticFileUrls("/css/index.min.css", true));
            if (appId != Config.App.AppId)
            {
                cssFiles.AddRange(GetStaticFileUrls($"/{appId}/page.min.css"));
            }
            jsFiles.AddRange(GetStaticFileUrls($"/js/lang/{appLang}.min.js", true));
            jsFiles.AddRange(GetStaticFileUrls("/js/index.min.js", true));
        }

        private static void InitPageFiles(string appId, string appLang, List<string> cssFiles, List<string> jsFiles)
        {
            cssFiles.AddRange(GetStaticFileUrls("/css/kui.min.css", true));
            jsFiles.AddRange(GetStaticFileUrls($"/js/lang/{appLang}.min.js", true));
            jsFiles.AddRange(GetStaticFileUrls("/js/kui.min.js", true));

            if (appId != SystemService.DevId)
            {
                cssFiles.AddRange(GetStaticFileUrls($"/{appId}/page.min.css"));
                jsFiles.AddRange(GetStaticFileUrls($"/{appId}/page.min.js"));
            }
        }

        private static string GetStaticUrl(string url)
        {
            var resUrl = "";// Config.App.CdnUrl;
            if (string.IsNullOrEmpty(resUrl))
                return $"{wwwrootName}{url}";

            return $"{resUrl}{url}";
        }

        private static List<string> GetStaticFileUrls(string url, bool isFrame = false)
        {
            var urls = new List<string>();
            url = url.Replace("~", "").ToLower();
            if (bundles == null || bundles.Count == 0)
            {
                urls.Add(GetStaticFileUrl(url, "", isFrame));
                return urls;
            }

            var bundle = bundles.FirstOrDefault(b => b.OutputFileName.EndsWith(url, true, null));
            if (bundle == null)
            {
                urls.Add(GetStaticFileUrl(url, "", isFrame));
                return urls;
            }

            foreach (var item in bundle.InputFiles)
            {
                if (item.Contains("*."))
                {
                    var path = Path.Combine(rootPath, item.Split('*')[0]);
                    var dirs = Directory.GetDirectories(path);
                    foreach (var dir in dirs)
                    {
                        var dirFiles = Directory.GetFiles(dir);
                        foreach (var file in dirFiles)
                        {
                            AddDevStaticFileUrl(urls, file);
                        }
                    }

                    var files = Directory.GetFiles(path);
                    foreach (var file in files)
                    {
                        AddDevStaticFileUrl(urls, file);
                    }
                }
                else
                {
                    var file = Path.Combine(rootPath, item);
                    AddDevStaticFileUrl(urls, file);
                }
            }

            return urls;
        }

        private static string GetStaticFileUrl(string url, string fileName = null, bool isFrame = false)
        {
            if (string.IsNullOrEmpty(fileName))
            {
#if NET6_0
                fileName = Path.Combine(url.Split('/'));
                fileName = Path.Combine(rootPath, "wwwroot", fileName);
#else
                fileName = url.TrimStart('/').Replace("/", "\\");
                fileName = Path.Combine(rootPath, fileName);
#endif
            }

            var fileInfo = new FileInfo(fileName);
            if (!fileInfo.Exists && isFrame)
                return GetStaticUrl(url);

            var time = fileInfo.LastWriteTime.ToString("yyMMddHHmmss");
            return $"{url}?v={time}";
        }

        private static void AddDevStaticFileUrl(List<string> urls, string fileName)
        {
#if NET6_0
            var srcPath = Path.Combine(rootPath, "src");
            var url = fileName.Replace(srcPath, "").Replace("\\", "/");
            var html = GetStaticFileUrl(url, fileName);
#else
            var srcPath = rootPath;
            var url = fileName.Replace(srcPath, "").Replace("\\", "/");
            var html = GetStaticFileUrl($"/{url}", fileName);
#endif
            if (!urls.Contains(html))
            {
                urls.Add(html);
            }
        }
    }

    class BundleConfig
    {
        public string OutputFileName { get; set; }
        public List<string> InputFiles { get; set; }

        internal static List<BundleConfig> Load()
        {
            var path = Path.Combine(Config.ContentRootPath, "bundleconfig.json");
            if (!File.Exists(path))
                return null;

            var json = File.ReadAllText(path);
            return Utils.FromJson<List<BundleConfig>>(json);
        }
    }
}