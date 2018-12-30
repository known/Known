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
            Displayer.WriteLine(ConsoleColor.Yellow, new string('-', 100));
            Displayer.WriteLine("|");
            Displayer.WriteLine($"|\t共有{types.Length}个测试类");
            Displayer.WriteLine("|");
            Displayer.WriteLine(ConsoleColor.Yellow, new string('-', 100));
            foreach (var type in types)
            {
                if (!type.Name.EndsWith("Test"))
                    continue;

                Displayer.WriteLine($"开始测试{type.Name}");
                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                foreach (var item in methods)
                {
                    Displayer.WriteLine($"开始测试 {item.Name}");
                    try
                    {
                        item.Invoke(null, null);
                    }
                    catch (Exception ex)
                    {
                        Displayer.WriteLine(ex.ToString());
                    }
                }
                Displayer.WriteLine(ConsoleColor.Yellow, new string('-', 100));
            }
        }
    }
}
