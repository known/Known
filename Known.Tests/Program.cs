using System;
using System.Reflection;

namespace Known.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            RunTest();
            Assert.DisplaySummary();
            Console.WriteLine("按任意键结束！");
            Console.ReadKey();
        }

        static void RunTest()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetExportedTypes();
            Assert.WriteLine(ConsoleColor.Yellow, new string('-', 100));
            Console.WriteLine("|");
            Console.WriteLine($"|\t共有{types.Length}个测试类");
            Console.WriteLine("|");
            Assert.WriteLine(ConsoleColor.Yellow, new string('-', 100));
            foreach (var type in types)
            {
                if (!type.Name.EndsWith("Test"))
                    continue;

                Console.WriteLine($"开始测试{type.Name}");
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
                Assert.WriteLine(ConsoleColor.Yellow, new string('-', 100));
            }
        }
    }
}
