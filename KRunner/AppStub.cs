using System;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;
using System.Text.RegularExpressions;

namespace KRunner
{
    class AppStub
    {
        public static string ServiceName;
        public static string ServiceDescription;

        public static void Start(string[] args, Action start, Action stop)
        {
            var cmd = string.Join(" ", args);
            if (args.Length >= 1)
            {
                var pattern = @"-(c|r|i|s)$";
                var flag1 = Regex.IsMatch(cmd, pattern, RegexOptions.IgnoreCase);
                if (flag1)
                {
                    var startType = args[0].Substring(1, 1).ToUpper();
                    switch (startType)
                    {
                        case "C":
                            StartByConsole(start, stop);
                            break;
                        case "I":
                            InstallService(ServiceName, ServiceDescription);
                            break;
                        case "R":
                            RemoveService(ServiceName);
                            break;
                        case "S":
                            ServiceBase.Run(new WinService(start, stop));
                            break;
                    }
                    return;
                }
            }

            Console.WriteLine("\r-[c|r|i]\r");
            Console.WriteLine("-c   run with console application\t");
            Console.WriteLine("-r   remove the windows service\t");
            Console.WriteLine("-i   install the windows service\t");
        }

        private static void StartByConsole(Action start, Action stop)
        {
            Console.WriteLine($"Welcome to {ServiceName}-{ServiceDescription}");
            Console.WriteLine("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
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
                                start.Invoke();
                                Console.WriteLine("JobRunner is started.");
                            }
                            break;
                        case "stop":
                        case "sp":
                            if (isStart)
                            {
                                isStart = false;
                                stop.Invoke();
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

        private static void InstallService(string serviceName, string serviceDescription)
        {
            if (ExistsService(serviceName))
                return;

            WinServiceInstaller.ServiceName = serviceName;
            WinServiceInstaller.ServiceDesc = serviceDescription;
            try
            {
                var exePath = Assembly.GetCallingAssembly().CodeBase;
                ManagedInstallerClass.InstallHelper(new[] { "/logfile=", "/InstallStateDir=", exePath });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void RemoveService(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
                return;

            if (!ExistsService(serviceName))
                return;

            WinServiceInstaller.ServiceName = serviceName;
            try
            {
                var exePath = Assembly.GetCallingAssembly().CodeBase;
                ManagedInstallerClass.InstallHelper(new[] { "/u", "/logfile=", "/InstallStateDir=", exePath });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static bool ExistsService(string serviceName)
        {
            var services = ServiceController.GetServices();
            foreach (var service in services)
            {
                if (serviceName.Equals(service.ServiceName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }
}
