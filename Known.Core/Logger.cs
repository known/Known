using System.Collections.Concurrent;

namespace Known.Core;

public interface ILogger
{
    void Error(Exception ex);
    void Error(string message);
    void Info(string message);
    void Debug(string message);
    void Flush();
}

public sealed class Logger
{
    private static readonly ILogger logger = new FileLogger();

    private Logger() { }

    public static ILogger GetLogger() => logger;
    public static void Error(string message) => logger.Error(message);
    public static void Info(string message) => logger.Info(message);
    public static void Debug(string message) => logger.Debug(message);
    public static void Flush() => logger.Flush();

    internal static void Exception(string type, string json, Exception ex)
    {
        Exception(new LogInfo
        {
            Message = ex.Message,
            StackTrace = string.Format("Type：{1}{0}Json：{2}{0}Error：{3}", Environment.NewLine, type, json, ex)
        }, ex);
    }

    public static void Exception(Exception ex)
    {
        Exception(new LogInfo
        {
            Message = ex.Message,
            StackTrace = ex.ToString()
        }, ex);
    }

    public static void Exception(LogInfo info, Exception ex)
    {
        info.System = Config.AppId;

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

class FileLogger : ILogger
{
    private static readonly ConcurrentQueue<string> errors = new();
    private static readonly ConcurrentQueue<string> infos = new();
    private static readonly ConcurrentQueue<string> debugs = new();

    static FileLogger()
    {
        var thread = new Thread(FlushQueue) { IsBackground = true };
        thread.Start();
    }

    public void Error(Exception ex) => Logger.Exception(ex);
    public void Error(string message) => errors.Enqueue(GetMessage("ERROR", message));
    public void Info(string message) => infos.Enqueue(GetMessage("INFO", message));
    public void Debug(string message) => debugs.Enqueue(GetMessage("DEBUG", message));
    public void Flush() => FlushLog();

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

    private static void WriteFile(string type, List<string> contents)
    {
        var path = Path.Combine(KCConfig.RootPath, "logs");
        path = Path.Combine(path, type);
        path = Path.Combine(path, $"{DateTime.Now:yyyyMMdd}.log");
        var info = new FileInfo(path);
        if (!info.Directory.Exists)
            info.Directory.Create();

        var text = string.Join(Environment.NewLine, contents.ToArray());
        File.AppendAllText(path, text);
    }
}