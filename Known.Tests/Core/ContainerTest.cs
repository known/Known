namespace Known.Tests.Core
{
    public class ContainerTest
    {
        public static void Register()
        {
            Container.Clear();
            Container.Register<ITestService, TestService>();

            var service = Container.Resolve<ITestService>();
            TestAssert.IsNotNull(service);
        }

        public static void RegisterInstance()
        {
            Container.Clear();

            var instance = new NameTestService("Known");
            Container.Register<INameTestService>(instance);

            var service = Container.Resolve<INameTestService>();
            TestAssert.IsNotNull(service);
        }

        public static void RegisterAssembly()
        {
            Container.Clear();
            Container.Register<BaseService>(typeof(ContainerTest).Assembly);

            var service = Container.Resolve<TestService>();
            TestAssert.IsNotNull(service);

            var nameService = Container.Resolve<NameTestService>();
            TestAssert.IsNotNull(nameService);
        }

        public static void RegisterAssemblyWithArgs()
        {
            Container.Clear();
            Container.Register<BaseService>(typeof(ContainerTest).Assembly, "Known");

            var service = Container.Resolve<TestService>();
            TestAssert.IsNotNull(service);

            var nameService = Container.Resolve<NameTestService>();
            TestAssert.IsNotNull(nameService);
        }

        public static void Resolve()
        {
            Container.Clear();
            Container.Register<ITestService, TestService>();

            var service = Container.Resolve<ITestService>();
            TestAssert.AreEqual(service.Hello(), "Hello!");
        }

        public static void ResolveInstance()
        {
            Container.Clear();

            var instance = new NameTestService("Known");
            Container.Register<INameTestService>(instance);

            var service = Container.Resolve<INameTestService>();
            TestAssert.AreEqual(service.Hello(), "Hello Known!");
        }

        public static void ResolveAssembly()
        {
            Container.Clear();
            Container.Register<BaseService>(typeof(ContainerTest).Assembly);

            var service = Container.Resolve<TestService>();
            TestAssert.AreEqual(service.Hello(), "Hello!");

            var nameService = Container.Resolve<NameTestService>();
            TestAssert.AreEqual(nameService.Hello(), "Hello!");
        }

        public static void ResolveAssemblyWithArgs()
        {
            Container.Clear();
            Container.Register<BaseService>(typeof(ContainerTest).Assembly, "Known");

            var service = Container.Resolve<TestService>();
            TestAssert.AreEqual(service.Hello(), "Hello!");

            var nameService = Container.Resolve<NameTestService>();
            TestAssert.AreEqual(nameService.Hello(), "Hello Known!");
        }
    }
}
