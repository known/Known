﻿namespace Known.Tests
{
    public class ObjectFactoryTest
    {
        public static void CreateService()
        {
            var context = Context.Create();
            var service = ObjectFactory.CreateService<ContextService>(context);
            TestAssert.IsNotNull(service);
            TestAssert.IsInstanceOf<ContextService>(service);
        }
    }
}
