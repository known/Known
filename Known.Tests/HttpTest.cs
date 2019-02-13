using System.Collections.Generic;
using Known.Web;

namespace Known.Tests
{
    class HttpTest
    {
        public static void FetchJgWeb()
        {
            var client = new HttpClient("http://localhost:83");
            var datas = new Dictionary<string, string>();
            var content = client.Get("/Home/Login?account=System&password=4a7d1ed414474e4033ac29ccb8653d9b");
            if (content != "3")
                return;

            content = client.Get("/Home/Main");
            var menus = client.Get("/Home/GetMenus");
        }
    }
}
