using Known.Core.Controllers;

namespace Known.Tests.Core
{
    public class IndexPageTest
    {
        private static readonly UserController user = new UserController();

        public static void Index()
        {
            var menus = user.GetModules();
            TestAssert.IsNotNull(menus);
        }

        public static void GetUser()
        {
            var info = user.GetData("1");
            TestAssert.IsNull(info);
        }

        public static void SignOut()
        {
            var res = user.SignOut();
            TestAssert.IsNotNull(res);
        }
    }
}
