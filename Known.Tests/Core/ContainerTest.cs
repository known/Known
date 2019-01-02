namespace Known.Tests.Core
{
    public class ContainerTest
    {
        public static void TestRegister()
        {
            Container.Register<ITestService, TestService>();
            var service = Container.Resolve<ITestService>();
            Assert.IsNotNull(service);
        }

        public static void TestRegisterInstance()
        {
            var instance = new NameTestService("Known");
            Container.Register<INameTestService, NameTestService>(instance);
            var service = Container.Resolve<ITestService>();
            Assert.IsNotNull(service);
        }

        public static void TestResolve()
        {
            Container.Register<ITestService, TestService>();
            var service = Container.Resolve<ITestService>();
            Assert.IsEqual(service.Hello(), "Hello!");

            var instance = new NameTestService("Known");
            Container.Register<INameTestService, NameTestService>(instance);
            var service1 = Container.Resolve<INameTestService>();
            Assert.IsEqual(service1.Hello(), "Hello Known!");
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

    public interface INameTestService
    {
        string Hello();
    }

    public class NameTestService : INameTestService
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
