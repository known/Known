using System;
using System.Reflection;
using Known.Data;
using Known.Mapping;

namespace Known.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            EntityHelper.InitMapper(typeof(Program).Assembly);

            RunTest();
            TestDisplayer.DisplaySummary();
            Console.WriteLine("按任意键结束！");
            Console.ReadKey();
        }

        static void RunTest()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetExportedTypes();
            TestDisplayer.WriteLine(ConsoleColor.Yellow, new string('-', 100));
            TestDisplayer.WriteLine("|");
            TestDisplayer.WriteLine($"|\t共有{types.Length}个测试类");
            TestDisplayer.WriteLine("|");
            TestDisplayer.WriteLine(ConsoleColor.Yellow, new string('-', 100));
            foreach (var type in types)
            {
                if (!type.Name.EndsWith("Test"))
                    continue;

                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                if (methods == null || methods.Length == 0)
                    continue;

                TestDisplayer.WriteLine($"开始测试{type.Name}");
                foreach (var item in methods)
                {
                    TestDisplayer.WriteLine($"开始测试 {item.Name}");
                    try
                    {
                        item.Invoke(null, null);
                    }
                    catch (Exception ex)
                    {
                        TestDisplayer.WriteLine(ex.ToString());
                    }
                }
                TestDisplayer.WriteLine(ConsoleColor.Yellow, new string('-', 100));
            }
        }
    }
}
