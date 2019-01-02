namespace Known.Tests.KnownTests
{
    public class ContainerTest
    {
        public static void TestRegister()
        {
            Container.Register<ITestService, TestService>();
            var service = Container.Resolve<ITestService>();
            Assert.IsNull(service);
        }

        public static void TestRegisterInstance()
        {
            var instance = new NameTestService("Known");
            Container.Register<ITestService, NameTestService>(instance);
            var service = Container.Resolve<ITestService>();
            Assert.IsNull(service);
        }

        public static void TestResolve()
        {
            Container.Register<ITestService, TestService>();
            var service = Container.Resolve<ITestService>();
            Assert.IsEqual(service.Hello(), "Hello!");

            var instance = new NameTestService("Known");
            Container.Register<ITestService, NameTestService>(instance);
            service = Container.Resolve<ITestService>();
            Assert.IsEqual(service.Hello(), "Hello Known!");
        }
    }

    public interface ITestService
    {
        string Hello();
    }

    public class TestService : ITestService
    {
        public string Hello()
        {
            return "Hello!";
        }
    }

    public class NameTestService : ITestService
    {
        private string name;

        public NameTestService(string name)
        {
            this.name = name;
        }

        public string Hello()
        {
            return $"Hello {name}!";
        }
    }
}
