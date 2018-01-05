using System;
using System.Reflection;

namespace Known.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            RunTest(typeof(KnownTests.ConfigTest));
            RunTest(typeof(KnownTests.UtilsTest));
            RunTest(typeof(KnownTests.LoggerTest));
            RunTest(typeof(KnownTests.ExtesionTest));
            RunTest(typeof(KnownTests.DataTest));

            Assert.DisplaySummary();
            Console.WriteLine("按任意键结束！");
            Console.ReadKey();
        }

        static void RunTest(Type type)
        {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (var item in methods)
            {
                Console.WriteLine($"开始测试 {item.Name}");
                try
                {
                    item.Invoke(null, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
