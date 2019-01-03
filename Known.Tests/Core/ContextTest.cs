using Known.Data;
using Known.Log;

namespace Known.Tests.Core
{
    public class ContextTest
    {
        public static void TestConstructor1()
        {
            var context = new Context(new ConsoleLogger());
            Assert.IsNotNull(context.Logger);
            Assert.IsNull(context.Database);
            Assert.IsNull(context.UserName);
            Assert.IsNotNull(context.Parameter);
        }

        public static void TestConstructor2()
        {
            var context = new Context(new Database(), new ConsoleLogger());
            Assert.IsNotNull(context.Logger);
            Assert.IsNotNull(context.Database);
            Assert.IsNull(context.UserName);
            Assert.IsNotNull(context.Parameter);
        }

        public static void TestConstructor3()
        {
            var context = new Context(new Database(), new ConsoleLogger(), "Known");
            Assert.IsNotNull(context.Logger);
            Assert.IsNotNull(context.Database);
            Assert.IsNotNull(context.UserName);
            Assert.IsNotNull(context.Parameter);
            Assert.IsEqual(context.UserName, "Known");
            Assert.IsEqual(context.Database.UserName, "Known");
        }

        public static void TestDynamicParam()
        {
            var context = new Context(new ConsoleLogger());
            context.Parameter.Id = 1;
            context.Parameter.Name = "Known";
            Assert.IsNotNull(context.Parameter);
            Assert.IsEqual(context.Parameter.Id, 1);
            Assert.IsEqual(context.Parameter.Name, "Known");
        }

        public static void TestCreate()
        {
            var context = Context.Create();

            Assert.IsNotNull(context);
            Assert.IsNotNull(context.Logger);
            Assert.IsNotNull(context.Database);
            Assert.IsNull(context.UserName);
            Assert.IsNotNull(context.Parameter);
        }

        public static void TestCreateWithArg()
        {
            var context = Context.Create("Known");

            Assert.IsNotNull(context);
            Assert.IsNotNull(context.Logger);
            Assert.IsNotNull(context.Database);
            Assert.IsNotNull(context.UserName);
            Assert.IsNotNull(context.Parameter);
            Assert.IsEqual(context.UserName, "Known");
            Assert.IsEqual(context.Database.UserName, "Known");
        }
    }
}
