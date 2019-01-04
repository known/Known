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

        public static void TestRegisterAssembly()
        {
            Container.Register<BaseService>(typeof(ContainerTest).Assembly);

            var service = Container.Resolve<TestService>();
            Assert.IsNotNull(service);

            var nameService = Container.Resolve<NameTestService>();
            Assert.IsNotNull(nameService);
        }

        public static void TestRegisterAssemblyWithArgs()
        {
            Container.Register<BaseService>(typeof(ContainerTest).Assembly, "Known");

            var service = Container.Resolve<TestService>();
            Assert.IsNotNull(service);

            var nameService = Container.Resolve<NameTestService>();
            Assert.IsNotNull(nameService);
        }

        public static void TestResolve()
        {
            Container.Register<ITestService, TestService>();

            var service = Container.Resolve<ITestService>();
            Assert.AreEqual(service.Hello(), "Hello!");
        }

        public static void TestResolveInstance()
        {
            var instance = new NameTestService("Known");
            Container.Register<INameTestService, NameTestService>(instance);

            var service = Container.Resolve<INameTestService>();
            Assert.AreEqual(service.Hello(), "Hello Known!");
        }

        public static void TestResolveAssembly()
        {
            Container.Register<BaseService>(typeof(ContainerTest).Assembly);

            var service = Container.Resolve<TestService>();
            Assert.AreEqual(service.Hello(), "Hello!");

            var nameService = Container.Resolve<NameTestService>();
            Assert.AreEqual(nameService.Hello(), "Hello!");
        }

        public static void TestResolveAssemblyWithArgs()
        {
            Container.Register<BaseService>(typeof(ContainerTest).Assembly, "Known");

            var service = Container.Resolve<TestService>();
            Assert.AreEqual(service.Hello(), "Hello!");

            var nameService = Container.Resolve<NameTestService>();
            Assert.AreEqual(nameService.Hello(), "Hello Known!");
        }
    }

    public abstract class BaseService
    {
    }

    public interface ITestService
    {
        string Hello();
    }

    public class TestService : BaseService, ITestService
    {
        private readonly string name;

        public TestService() { }

        public TestService(string name)
        {
            this.name = name;
        }

        public string Hello()
        {
            return "Hello!";
        }
    }

    public interface INameTestService
    {
        string Hello();
    }

    public class NameTestService : BaseService, INameTestService
    {
        private readonly string name;

        public NameTestService() { }

        public NameTestService(string name)
        {
            this.name = name;
        }

        public string Hello()
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Hello!";

            return $"Hello {name}!";
        }
    }
}
