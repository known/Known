using System;
using System.Collections.Generic;
using System.IO;
using Known.Extensions;
using Known.Web;

namespace Known.Tests
{
    class HttpTest
    {
        class Menu
        {
            public string oid { get; set; }
            public string code { get; set; }
            public string name { get; set; }
            public string parentoid { get; set; }
            public string url { get; set; }
            public string img { get; set; }
        }

        public static void FetchWeb()
        {
            FetchWeb("http://localhost:83", @"D:\Known\Projects\KPlatform\demo\jg");
            FetchWeb("http://localhost:72", @"D:\Known\Projects\KPlatform\demo\zdz");
            FetchWeb("http://localhost:71", @"D:\Known\Projects\KPlatform\demo\cjlr");
        }

        public static void FetchZgWeb()
        {
            FetchWeb("http://localhost:81", @"D:\Known\Projects\KPlatform\demo\bg");
        }

        public static void FetchXtWeb()
        {
            FetchWeb("http://localhost:82", @"D:\Known\Projects\KPlatform\demo\xt");
        }

        private static void FetchWeb(string baseUrl, string basePath)
        {
            Console.WriteLine("开始获取网页....");
            var client = new HttpClient(baseUrl);
            var datas = new Dictionary<string, string>();
            var content = client.Get("/Home/Login?account=System&password=4a7d1ed414474e4033ac29ccb8653d9b");
            content = client.Get("/Home/Main");
            content = client.Get("/Home/GetMenus");
            var menuPath = $@"{basePath}\Data\menus.json";
            Utils.EnsureFile(menuPath);
            File.WriteAllText(menuPath, content);

            var menus = content.FromJson<List<Menu>>();
            foreach (var item in menus)
            {
                if (!string.IsNullOrWhiteSpace(item.url))
                {
                    Console.WriteLine($"获取{item.name}-{item.url}");
                    client.SetCookie("ModuleId", item.oid);
                    content = client.Get(item.url, true);
                    var fileName = item.url.Replace("/", "\\")
                                           .Replace("?", "_")
                                           .Replace("&", "_")
                                           .Replace("=", "_");
                    var filePath = $@"{basePath}\Pages\{fileName}.html";
                    Utils.EnsureFile(filePath);
                    File.WriteAllText(filePath, content);
                }
            }

            Console.WriteLine("获取结束！");
        }
    }
}
