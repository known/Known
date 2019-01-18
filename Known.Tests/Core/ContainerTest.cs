namespace Known.Tests.Core
{
    public class ContainerTest
    {
        public static void Register()
        {
            Container.Register<ITestService, TestService>();

            var service = Container.Resolve<ITestService>();
            TestAssert.IsNotNull(service);
        }

        public static void RegisterInstance()
        {
            var instance = new NameTestService("Known");
            Container.Register<INameTestService, NameTestService>(instance);

            var service = Container.Resolve<ITestService>();
            TestAssert.IsNotNull(service);
        }

        public static void RegisterAssembly()
        {
            Container.Register<BaseService>(typeof(ContainerTest).Assembly);

            var service = Container.Resolve<TestService>();
            TestAssert.IsNotNull(service);

            var nameService = Container.Resolve<NameTestService>();
            TestAssert.IsNotNull(nameService);
        }

        public static void RegisterAssemblyWithArgs()
        {
            Container.Register<BaseService>(typeof(ContainerTest).Assembly, "Known");

            var service = Container.Resolve<TestService>();
            TestAssert.IsNotNull(service);

            var nameService = Container.Resolve<NameTestService>();
            TestAssert.IsNotNull(nameService);
        }

        public static void Resolve()
        {
            Container.Register<ITestService, TestService>();

            var service = Container.Resolve<ITestService>();
            TestAssert.AreEqual(service.Hello(), "Hello!");
        }

        public static void ResolveInstance()
        {
            var instance = new NameTestService("Known");
            Container.Register<INameTestService, NameTestService>(instance);

            var service = Container.Resolve<INameTestService>();
            TestAssert.AreEqual(service.Hello(), "Hello Known!");
        }

        public static void ResolveAssembly()
        {
            Container.Register<BaseService>(typeof(ContainerTest).Assembly);

            var service = Container.Resolve<TestService>();
            TestAssert.AreEqual(service.Hello(), "Hello!");

            var nameService = Container.Resolve<NameTestService>();
            TestAssert.AreEqual(nameService.Hello(), "Hello!");
        }

        public static void ResolveAssemblyWithArgs()
        {
            Container.Register<BaseService>(typeof(ContainerTest).Assembly, "Known");

            var service = Container.Resolve<TestService>();
            TestAssert.AreEqual(service.Hello(), "Hello!");

            var nameService = Container.Resolve<NameTestService>();
            TestAssert.AreEqual(nameService.Hello(), "Hello Known!");
        }
    }
}
