using System;
#if !NET35
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Known.Core;

namespace Known
{
    public interface ILogger
    {
        void Error(string message, Exception ex);
        void Error(string message);
        void Info(string message);
        void Debug(string message);
        void Flush();
    }

    public sealed class Logger
    {
        private static readonly ILogger logger = new FileLogger();

        private Logger() { }

        public static ILogger GetLogger()
        {
            return logger;
        }

        public static void Error(string message)
        {
            logger.Error(message);
        }

        public static void Info(string message)
        {
            logger.Info(message);
        }

        public static void Debug(string message)
        {
            logger.Debug(message);
        }

        public static void Flush()
        {
            logger.Flush();
        }

        internal static void Exception(string type, string json, Exception ex)
        {
            Exception(new LogInfo
            {
                Message = ex.Message,
                StackTrace = string.Format("Type：{1}{0}Json：{2}{0}Error：{3}", Environment.NewLine, type, json, ex)
            }, ex);
        }

        public static void Exception(string message, Exception ex)
        {
            Exception(new LogInfo
            {
                Message = $"{message}: {ex.Message}",
                StackTrace = ex.ToString()
            }, ex);
        }

        public static void Exception(LogInfo info, Exception ex)
        {
            var app = Config.App;
            if (app != null)
                info.System = app.AppId;

            if (string.IsNullOrEmpty(info.Message))
                info.Message = ex.Message;
            if (string.IsNullOrEmpty(info.StackTrace))
                info.StackTrace = ex.ToString();

            Error($"{info.User} {info.Url}{Environment.NewLine}{info.StackTrace}");
        }
    }

    public class LogInfo
    {
        public string Id { get; set; }
        public DateTime CreateTime { get; set; }
        public string System { get; set; }
        public string User { get; set; }
        public string IP { get; set; }
        public string IPName { get; set; }
        public string Url { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }

    sealed class ErrorHelper
    {
        private ErrorHelper() { }

        internal static List<LogInfo> Errors { get; } = new List<LogInfo>();

        internal static void AddError(LogInfo error)
        {
            var exist = Errors.FirstOrDefault(l => l.Id == error.Id);
            if (exist == null)
                exist = error;
            else
                Errors.Add(error);
        }

        internal static void RemoveError(string id)
        {
            var error = Errors.FirstOrDefault(l => l.Id == id);
            if (error == null)
                return;

            Errors.Remove(error);
        }
    }

    class FileLogger : ILogger
    {
#if NET35
        private static readonly Queue<string> errors = new Queue<string>();
        private static readonly Queue<string> infos = new Queue<string>();
        private static readonly Queue<string> debugs = new Queue<string>();
#else
        private static readonly ConcurrentQueue<string> errors = new ConcurrentQueue<string>();
        private static readonly ConcurrentQueue<string> infos = new ConcurrentQueue<string>();
        private static readonly ConcurrentQueue<string> debugs = new ConcurrentQueue<string>();
#endif

        static FileLogger()
        {
            var thread = new Thread(FlushQueue) { IsBackground = true };
            thread.Start();
        }

        public void Error(string message, Exception ex)
        {
            Logger.Exception(message, ex);
        }

        public void Error(string message)
        {
            var item = GetMessage("ERROR", message);
            errors.Enqueue(item);
        }

        public void Info(string message)
        {
            var item = GetMessage("INFO", message);
            infos.Enqueue(item);
        }

        public void Debug(string message)
        {
            var item = GetMessage("DEBUG", message);
            debugs.Enqueue(item);
        }

        public void Flush()
        {
            FlushLog();
        }

        private static string GetMessage(string type, string message)
        {
            var text = $"{DateTime.Now:yyyy-MM-dd.HH:mm:ss.fff} {type} {message}";
            Console.WriteLine(text);
            return text;
        }

        private static void FlushQueue()
        {
            while (true)
            {
                FlushLog();
                Thread.Sleep(5000);
            }
        }

#if NET35
        private static void FlushLog()
        {
            if (errors.Count > 0)
                WriteLog("Errors", errors);

            if (infos.Count > 0)
                WriteLog("Infos", infos);

            if (debugs.Count > 0)
                WriteLog("Debugs", debugs);
        }

        private static void WriteLog(string type, Queue<string> items)
        {
            var contents = new List<string>();
            while (true)
            {
                var item = items.Dequeue();
                contents.Add(item);

                if (items.Count == 0)
                    break;
            }

            WriteFile(type, contents);
        }
#else
        private static void FlushLog()
        {
            if (!errors.IsEmpty)
                WriteLog("Errors", errors);

            if (!infos.IsEmpty)
                WriteLog("Infos", infos);

            if (!debugs.IsEmpty)
                WriteLog("Debugs", debugs);
        }

        private static void WriteLog(string type, ConcurrentQueue<string> items)
        {
            var contents = new List<string>();
            while (true)
            {
                if (items.TryDequeue(out string item))
                    contents.Add(item);

                if (items.IsEmpty)
                    break;
            }

            WriteFile(type, contents);
        }
#endif

        private static void WriteFile(string type, List<string> contents)
        {
            var path = Path.Combine(Config.RootPath, "logs");
            path = Path.Combine(path, type);
            path = Path.Combine(path, $"{DateTime.Now:yyyyMMdd}.log");
            var info = new FileInfo(path);
            if (!info.Directory.Exists)
                info.Directory.Create();

            var text = string.Join(Environment.NewLine, contents.ToArray());
            File.AppendAllText(path, text);
        } 
    }
}