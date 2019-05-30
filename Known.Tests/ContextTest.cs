using Known.Data;
using Known.Log;

namespace Known.Tests
{
    public class ContextTest
    {
        public static void Constructor1()
        {
            var context = new Context(new ConsoleLogger());
            TestAssert.IsNotNull(context.Logger);
            TestAssert.IsNull(context.Database);
            TestAssert.IsNull(context.UserName);
            TestAssert.IsNotNull(context.Parameter);
        }

        public static void Constructor2()
        {
            var context = new Context(new Database(), new ConsoleLogger());
            TestAssert.IsNotNull(context.Logger);
            TestAssert.IsNotNull(context.Database);
            TestAssert.IsNull(context.UserName);
            TestAssert.IsNotNull(context.Parameter);
        }

        public static void Constructor3()
        {
            var context = new Context(new Database(), new ConsoleLogger(), "Known");
            TestAssert.IsNotNull(context.Logger);
            TestAssert.IsNotNull(context.Database);
            TestAssert.IsNotNull(context.UserName);
            TestAssert.IsNotNull(context.Parameter);
            TestAssert.AreEqual(context.UserName, "Known");
            TestAssert.AreEqual(context.Database.UserName, "Known");
        }

        public static void DynamicParam()
        {
            var context = new Context(new ConsoleLogger());
            context.Parameter.Id = 1;
            context.Parameter.Name = "Known";
            TestAssert.IsNotNull(context.Parameter);
            TestAssert.AreEqual(context.Parameter.Id, 1);
            TestAssert.AreEqual(context.Parameter.Name, "Known");
        }

        public static void Create()
        {
            var context = Context.Create();

            TestAssert.IsNotNull(context);
            TestAssert.IsNotNull(context.Logger);
            TestAssert.IsNotNull(context.Database);
            TestAssert.IsNull(context.UserName);
            TestAssert.IsNotNull(context.Parameter);
        }

        public static void CreateWithArg()
        {
            var context = Context.Create("Known");

            TestAssert.IsNotNull(context);
            TestAssert.IsNotNull(context.Logger);
            TestAssert.IsNotNull(context.Database);
            TestAssert.IsNotNull(context.UserName);
            TestAssert.IsNotNull(context.Parameter);
            TestAssert.AreEqual(context.UserName, "Known");
            TestAssert.AreEqual(context.Database.UserName, "Known");
        }
    }
}
