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

        public static void FetchZjgWeb()
        {
            FetchWeb("http://localhost:83", @"D:\Known\Projects\ZjgClient");
        }

        public static void FetchZdzWeb()
        {
            FetchWeb("http://localhost:72", @"D:\Known\Projects\ZdzClient");
        }

        private static void FetchWeb(string baseUrl, string basePath)
        {
            Console.WriteLine("开始获取网页....");
            var client = new HttpClient(baseUrl);
            var datas = new Dictionary<string, string>();
            var content = client.Get("/Home/Login?account=System&password=4a7d1ed414474e4033ac29ccb8653d9b");
            if (content != "3")
                return;

            content = client.Get("/Home/Main");
            content = client.Get("/Home/GetMenus");
            File.WriteAllText($@"{basePath}\Data\menus.json", content);

            var menus = content.FromJson<List<Menu>>();
            foreach (var item in menus)
            {
                if (!string.IsNullOrWhiteSpace(item.url))
                {
                    Console.WriteLine($"获取{item.name}-{item.url}");
                    content = client.Get(item.url);
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
