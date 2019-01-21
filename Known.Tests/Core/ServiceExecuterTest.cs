namespace Known.Tests.Core
{
    public class ServiceExecuterTest
    {
        public static void Execute()
        {
            Container.Clear();
            Container.Register<ServiceBase>(typeof(Program).Assembly, Context.Create());

            var executer = new ServiceExecuter("Known", "Context", "SayHello");
            var value = executer.Execute();
            TestAssert.AreEqual(value, "Hello!");

            executer.Method = "SayHelloByName";
            executer.Parameter = "Known";
            value = executer.Execute();
            TestAssert.AreEqual(value, "Hello Known!");

            executer.Method = "SayHelloByGreet";
            executer.Parameter = null;
            executer.Parameters.Clear();
            executer.Parameters.Add("greet", "Great");
            executer.Parameters.Add("name", "Known");
            value = executer.Execute();
            TestAssert.AreEqual(value, "Hello Known, Great!");
        }
    }
}
