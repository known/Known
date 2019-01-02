using Known.Log;

namespace Known.Tests.Core
{
    public class BusinessTest
    {
        public static void TestConstructor()
        {
            var context = new Context(new ConsoleLogger());
            //var business = new BusinessBase(context);
            //Assert.IsNotNull(business.Context);
            //Assert.IsNotNull(business.Context.Logger);
            //Assert.IsNull(business.Context.Database);
            //Assert.IsNull(business.Context.UserName);
            //Assert.IsNull(business.Context.Param);
        }
    }
}
