using Known.Core.Controllers;

namespace Known.Tests.Core
{
    public class LoginPageTest
    {
        public static void Login()
        {
            var controller = new UserController();
            var res = controller.SignIn("admin", "1");
            TestAssert.IsNotNull(res);
        }
    }
}
