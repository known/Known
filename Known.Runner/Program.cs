using System;
using System.IO;

namespace Known.Runner
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (o, e) => Logger.Fatal(e.ExceptionObject);

            Console.WriteLine("Start loading config...");
            var file = new FileInfo("app.json");
            var info = AppInfo.Load(file);
            if (info == null)
            {
                Console.WriteLine("The app.json is not exists.");
                return;
            }

            if (info.Jobs == null || info.Jobs.Count == 0)
            {
                Console.WriteLine("No ThreadJob to run.");
                return;
            }

            foreach (var item in info.Jobs)
            {
                var type = Type.GetType(item.TypeName);
                if (type == null)
                {
                    Console.WriteLine($"The {item.TypeName} is not exists.");
                    continue;
                }

                var job = Activator.CreateInstance(type) as IThreadJob;
                if (job == null)
                {
                    Console.WriteLine($"The {item.TypeName} is not impl the IThreadJob.");
                    continue;
                }

                Console.WriteLine($"{item.Name} is running.");
                job.Config = item;
                job.Run();
            }
        }
    }
}
