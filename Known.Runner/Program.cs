using System;
using System.IO;

namespace Known.Runner
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
            AppStub.Start(args, () => JobRunner.Start(info), () => JobRunner.Stop());            
        }
    }

    class TestJob : IThreadJob
    {
        public JobConfig Config { get; set; }

        public void Run()
        {
            Console.WriteLine(Config.TypeName);
        }
    }
}
