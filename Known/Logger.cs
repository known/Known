using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Known
{
    public sealed class Logger
    {
        private static readonly ConcurrentQueue<string> errors = new ConcurrentQueue<string>();
        private static readonly ConcurrentQueue<string> infos = new ConcurrentQueue<string>();
        private Logger() { }

        static Logger()
        {
            var thread = new Thread(Flush) { IsBackground = true };
            thread.Start();
        }

        private static void Flush()
        {
            while (true)
            {
                if (errors.Count > 0)
                    WriteLog("Errors", errors);

                if (infos.Count > 0)
                    WriteLog("Infos", infos);

                Thread.Sleep(5000);
            }
        }

        private static void WriteLog(string type, ConcurrentQueue<string> items)
        {
            var contents = new List<string>();
            while (true)
            {
                if (items.TryDequeue(out string item))
                    contents.Add(item);

                if (items.Count == 0)
                    break;
            }

            var path = Path.Combine(Config.RootPath, "log", type, $"{DateTime.Now:yyyyMMdd}.log");
            var info = new FileInfo(path);
            if (!info.Directory.Exists)
                info.Directory.Create();

            File.AppendAllLines(path, contents);
        }

        public static void Error(string message)
        {
            var item = GetMessage("ERROR", message);
            errors.Enqueue(item);
        }

        public static void Info(string message)
        {
            var item = GetMessage("INFO", message);
            infos.Enqueue(item);
        }

        private static string GetMessage(string type, string message)
        {
            var text = $"{DateTime.Now:yyyy-MM-dd.HH:mm:ss.fff} {type}:{message}";
            Console.WriteLine(text);
            return text;
        }
    }
}
