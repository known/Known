using System.Reflection;

namespace Known.Tests
{
    public class ContainerTest
    {
        public static void Register()
        {
            Container.Remove<ITestService>();
            Container.Register<ITestService, TestService>();

            var service = Container.Resolve<ITestService>();
            TestAssert.IsNotNull(service);
        }

        public static void RegisterInstance()
        {
            Container.Remove<INameTestService>();

            var instance = new NameTestService("Known");
            Container.Register<INameTestService>(instance);

            var service = Container.Resolve<INameTestService>();
            TestAssert.IsNotNull(service);
        }

        public static void RegisterAssembly()
        {
            var assembly = typeof(ContainerTest).Assembly;
            ClearRegisterAssembly<BaseService>(assembly);

            Container.Register<BaseService>(assembly);

            var service = Container.Resolve<TestService>();
            TestAssert.IsNotNull(service);

            var nameService = Container.Resolve<NameTestService>();
            TestAssert.IsNotNull(nameService);
        }

        public static void RegisterAssemblyWithArgs()
        {
            var assembly = typeof(ContainerTest).Assembly;
            ClearRegisterAssembly<BaseService>(assembly);

            Container.Register<BaseService>(assembly, "Known");

            var service = Container.Resolve<TestService>();
            TestAssert.IsNotNull(service);

            var nameService = Container.Resolve<NameTestService>();
            TestAssert.IsNotNull(nameService);
        }

        public static void Resolve()
        {
            Container.Remove<ITestService>();
            Container.Register<ITestService, TestService>();

            var service = Container.Resolve<ITestService>();
            TestAssert.AreEqual(service.Hello(), "Hello!");
        }

        public static void ResolveInstance()
        {
            Container.Remove<INameTestService>();

            var instance = new NameTestService("Known");
            Container.Register<INameTestService>(instance);

            var service = Container.Resolve<INameTestService>();
            TestAssert.AreEqual(service.Hello(), "Hello Known!");
        }

        public static void ResolveAssembly()
        {
            var assembly = typeof(ContainerTest).Assembly;
            ClearRegisterAssembly<BaseService>(assembly);

            Container.Register<BaseService>(assembly);

            var service = Container.Resolve<TestService>();
            TestAssert.AreEqual(service.Hello(), "Hello!");

            var nameService = Container.Resolve<NameTestService>();
            TestAssert.AreEqual(nameService.Hello(), "Hello!");
        }

        public static void ResolveAssemblyWithArgs()
        {
            var assembly = typeof(ContainerTest).Assembly;
            ClearRegisterAssembly<BaseService>(assembly);

            Container.Register<BaseService>(assembly, "Known");

            var service = Container.Resolve<TestService>();
            TestAssert.AreEqual(service.Hello(), "Hello!");

            var nameService = Container.Resolve<NameTestService>();
            TestAssert.AreEqual(nameService.Hello(), "Hello Known!");
        }

        private static void ClearRegisterAssembly<T>(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(T)) && !type.IsAbstract)
                {
                    Container.Remove(type.Name);
                }
            }
        }
    }
}
