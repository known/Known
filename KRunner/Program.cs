using Known;
using System;
using System.IO;

namespace KRunner
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var file = new FileInfo("app.json");
            var info = AppInfo.Load(file);
            if (info == null)
            {
                Console.WriteLine("The app.json is not exists.");
                return;
            }

            AppDomain.CurrentDomain.UnhandledException += (o, e) => Logger.Fatal(e.ExceptionObject);
            AppStub.ServiceName = info.Name;
            AppStub.ServiceDescription = info.Description;
            args = new string[] { "-c" };
            AppStub.Start(args, () => JobRunner.Start(info), () => JobRunner.Stop());            
        }
    }

    class TestJob : JobBase, IJob
    {
        protected override void Runing()
        {
            Console.WriteLine("{0:yyyy-MM-dd HH:mm:ss} {1} {2}", DateTime.Now, Config.Name, Config.TypeName);
        }
    }
}
