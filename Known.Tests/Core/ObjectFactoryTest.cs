namespace Known.Tests.Core
{
    public class ObjectFactoryTest
    {
        public static void CreateService()
        {
            ObjectFactory.Clear();
            var context = Context.Create();
            var service = ObjectFactory.CreateService<ContextService>(context);
            TestAssert.IsNotNull(service);
            TestAssert.IsInstanceOf<ContextService>(service);
        }

        public static void CreateRepository()
        {
            ObjectFactory.Clear();
            var context = Context.Create();
            var repository = ObjectFactory.CreateRepository<IContextRepository>(context);
            TestAssert.IsNotNull(repository);

            Container.Clear();
            Container.Register<IContextRepository, ContextRepository>();
            repository = ObjectFactory.CreateRepository<IContextRepository>(context);
            TestAssert.IsNotNull(repository);
            TestAssert.IsInstanceOf<ContextRepository>(repository);
        }
    }
}
