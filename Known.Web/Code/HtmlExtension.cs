using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Known.Extensions;

namespace Known.Web
{
    public static class HtmlExtension
    {
        public static IHtmlString MinStyle(this HtmlHelper helper, string path)
        {
            var format = "<link rel=\"stylesheet\" href=\"{0}\">";
            var html = GetHtmlString(helper, format, path);
            return new HtmlString(html);
        }

        public static IHtmlString MinScript(this HtmlHelper helper, string path)
        {
            var format = "<script src=\"{0}\"></script>";
            var html = GetHtmlString(helper, format, path);
            return new HtmlString(html);
        }

        private static string GetHtmlString(HtmlHelper helper, string format, string path)
        {
            var random = DateTime.Now.ToString("yyMMddss");
            var html = string.Format(format, $"{path}?r={random}");
            var httpContext = helper.ViewContext.RequestContext.HttpContext;
            if (httpContext.IsDebuggingEnabled)
            {
                var bundle = BundleInfo.GetBundle(httpContext, path);
                if (bundle != null && bundle.HasInputFiles)
                {
                    var paths = new List<string>();
                    foreach (var inputFile in bundle.inputFiles)
                    {
                        if (File.Exists(inputFile))
                        {
                            paths.Add(string.Format(format, $"{inputFile}?r={random}"));
                        }
                        else if (Directory.Exists(inputFile))
                        {
                            var files = Directory.GetFiles(inputFile);
                            foreach (var file in files)
                            {
                                var filePath = file.Replace("\\", "/");
                                paths.Add(string.Format(format, $"{filePath}?r={random}"));
                            }
                        }
                    }
                    html = string.Join(Environment.NewLine, paths);
                }
            }

            return html;
        }

        class BundleInfo
        {
            public string outputFileName { get; set; }
            public List<string> inputFiles { get; set; }

            public bool HasInputFiles
            {
                get { return inputFiles != null && inputFiles.Count > 0; }
            }

            public static BundleInfo GetBundle(HttpContextBase httpContext, string outputFile)
            {
                var jsonFile = httpContext.Server.MapPath("~/bundleconfig.json");
                if (!File.Exists(jsonFile))
                    return null;

                var json = File.ReadAllText(jsonFile);
                if (string.IsNullOrWhiteSpace(json))
                    return null;

                var bundles = json.FromJson<List<BundleInfo>>();
                if (bundles == null || bundles.Count == 0)
                    return null;

                return bundles.FirstOrDefault(b => b.outputFileName == outputFile);
            }
        }
    }
}