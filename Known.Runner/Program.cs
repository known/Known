using System;

namespace Known.Runner
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (o, e) => Logger.Fatal(e.ExceptionObject);

            bool isStart = false;
            while (true)
            {
                Console.WriteLine("--->");
                var cmd = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(cmd))
                {
                    switch (cmd.ToLower())
                    {
                        case "start":
                        case "st":
                            if (!isStart)
                            {
                                isStart = true;
                                JobRunner.Start();
                                Console.WriteLine("JobRunner is started.");
                            }
                            break;
                        case "stop":
                        case "sp":
                            if (isStart)
                            {
                                isStart = false;
                                JobRunner.Stop();
                                Console.WriteLine("JobRunner is stoped.");
                            }
                            break;
                        case "clear":
                        case "cr":
                            Console.Clear();
                            break;
                        case "exit":
                        case "et":
                            return;
                        default:
                            Console.WriteLine("start\t(st)\nstop\t(sp)\nclear\t(cr)\nexit\t(et)");
                            break;
                    }
                }
            }
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
