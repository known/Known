using System.Collections.Generic;

namespace Known.Tests
{
    public class ServiceExecuterTest
    {
        public static void Execute()
        {
            Container.Remove<ServiceBase>();
            Container.Register<ServiceBase>(typeof(Program).Assembly, Context.Create());

            var executer = new ServiceExecuter("Known", "Context", "SayHello");
            var value = executer.Execute(null);
            TestAssert.AreEqual(value, "Hello!");

            executer.Method = "SayHelloByName";
            value = executer.Execute("Known");
            TestAssert.AreEqual(value, "Hello Known!");

            executer.Method = "SayHelloByGreet";
            var parameters = new Dictionary<string, object>();
            parameters.Add("greet", "Great");
            parameters.Add("name", "Known");
            value = executer.Execute(parameters);
            TestAssert.AreEqual(value, "Hello Known, Great!");
        }
    }
}
