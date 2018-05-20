using Known.Log;

namespace Known.Tests.KnownTests
{
    public class ContextTest
    {
        public static void TestConstructor1()
        {
            var context = new Context(new ConsoleLogger());
            Assert.IsNotNull(context.Logger);
            Assert.IsNull(context.Database);
            Assert.IsNull(context.UserName);
            Assert.IsNull(context.Param);
        }

        public static void TestConstructor2()
        {
            var context = new Context(Config.GetDatabase(), new ConsoleLogger());
            Assert.IsNotNull(context.Logger);
            Assert.IsNotNull(context.Database);
            Assert.IsNull(context.UserName);
            Assert.IsNull(context.Param);
        }

        public static void TestConstructor3()
        {
            var context = new Context(Config.GetDatabase(), new ConsoleLogger(), "Known");
            Assert.IsNotNull(context.Logger);
            Assert.IsNotNull(context.Database);
            Assert.IsNotNull(context.UserName);
            Assert.IsNull(context.Param);
            Assert.IsEqual(context.UserName, "Known");
            Assert.IsEqual(context.Database.UserName, "Known");
        }

        public static void TestDynamicParam()
        {
            var context = new Context(new ConsoleLogger())
            {
                Param = new { Id = 1, Name = "Known" }
            };
            Assert.IsNotNull(context.Param);
            Assert.IsEqual(context.Param.Id, 1);
            Assert.IsEqual(context.Param.Name, "Known");
        }
    }
}
