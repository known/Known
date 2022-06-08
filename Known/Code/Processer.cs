using System.Diagnostics;
using System.Threading;

namespace Known
{
    public class StartResult
    {
        public int Id { get; set; }
        public bool IsSuccess { get; set; }
        public string ProcessName { get; set; }
        public string FileVersion { get; set; }
    }

    public sealed class Processer
    {
        private Processer() { }

        public static StartResult Start(string exeFile, string arg = null)
        {
            var process = new Process();
            process.StartInfo.FileName = exeFile;
            if (!string.IsNullOrEmpty(arg))
            {
                process.StartInfo.Arguments = arg;
            }
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();

            bool isSuccess;
            while (true)
            {
                isSuccess = Exists(process.ProcessName, exeFile);
                if (isSuccess)
                    break;
                Thread.Sleep(500);
            }

            return new StartResult
            {
                Id = process.Id,
                IsSuccess = isSuccess,
                ProcessName = process.ProcessName,
                FileVersion = process.MainModule.FileVersionInfo.FileVersion
            };
        }

        public static void Stop(string processName, string fileName)
        {
            if (string.IsNullOrEmpty(processName))
                return;

            var lists = Process.GetProcessesByName(processName);
            foreach (var item in lists)
            {
                if (item.MainModule.FileName == fileName)
                {
                    item.Kill();
                }
            }
        }

        public static bool Exists(string processName, string fileName)
        {
            if (string.IsNullOrEmpty(processName))
                return false;

            var lists = Process.GetProcessesByName(processName);
            foreach (var item in lists)
            {
                if (item.MainModule.FileName == fileName)
                {
                    return true;
                }
            }

            return false;
        }
    }
}