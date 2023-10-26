using System.Collections.Concurrent;
using Known.Repositories;

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

    public static async Task<List<string>> GetVisitMenuIdsAsync(Database db, string userName, int size)
    {
        var logs = await LogRepository.GetLogCountsAsync(db, userName, Constants.LogTypePage);
        logs = logs.OrderByDescending(f => f.TotalCount).Take(size).ToList();
        return logs.Select(l => l.Field1).ToList();
    }

    public static Task AddLogAsync(Database db, string type, string target, string content)
    {
        return db.SaveAsync(new SysLog
        {
            Type = type,
            Target = target,
            Content = content
        });
    }

    public static ILogger GetLogger() => logger;
    public static void Error(string message) => logger.Error(message);
    public static void Info(string message) => logger.Info(message);
    public static void Debug(string message) => logger.Debug(message);
    public static void Flush() => logger.Flush();

    public static void Exception(Exception ex)
    {
        Error(ex.ToString());
    }
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