using Known.Core.Controllers;

namespace Known.Tests.Core
{
    public class LoginPageTest
    {
        public static void SignIn()
        {
            var user = new UserController();
            var res = user.SignIn("admin", "1");
            TestAssert.IsNotNull(res);
        }
    }
}
