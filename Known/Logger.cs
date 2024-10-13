namespace Known;

/// <summary>
/// 日志接口。
/// </summary>
public interface ILogger
{
    /// <summary>
    /// 错误日志。
    /// </summary>
    /// <param name="message">错误信息。</param>
    void Error(string message);

    /// <summary>
    /// 信息日志。
    /// </summary>
    /// <param name="message">提示信息。</param>
    void Info(string message);

    /// <summary>
    /// 调试日志。
    /// </summary>
    /// <param name="message">调试信息。</param>
    void Debug(string message);

    /// <summary>
    /// 刷新日志缓冲区。
    /// </summary>
    void Flush();
}

/// <summary>
/// 日志类型。
/// </summary>
public enum LogType
{
    /// <summary>
    /// 登录。
    /// </summary>
    Login,
    /// <summary>
    /// 退出。
    /// </summary>
    Logout,
    /// <summary>
    /// 页面访问。
    /// </summary>
    Page
}

/// <summary>
/// 日志级别。
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// 错误。
    /// </summary>
    Error,
    /// <summary>
    /// 信息。
    /// </summary>
    Info,
    /// <summary>
    /// 调试。
    /// </summary>
    Debug
}

/// <summary>
/// 日志操作类。
/// </summary>
public sealed class Logger
{
    private static readonly FileLogger logger = new();

    private Logger() { }

    /// <summary>
    /// 取得或设置全局日志级别。
    /// </summary>
    public static LogLevel Level { get; set; }

    /// <summary>
    /// 添加系统日志信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="type">日志类型。</param>
    /// <param name="target">操作目标。</param>
    /// <param name="content">操作内容。</param>
    /// <returns></returns>
    public static Task AddLogAsync(Database db, string type, string target, string content)
    {
        return db.SaveAsync(new SysLog
        {
            Type = type,
            Target = target,
            Content = content
        });
    }

    /// <summary>
    /// 错误日志。
    /// </summary>
    /// <param name="message">错误信息。</param>
    public static void Error(string message) => logger.Error(message);

    /// <summary>
    /// 信息日志。
    /// </summary>
    /// <param name="message">提示信息。</param>
    public static void Info(string message)
    {
        if (Level > LogLevel.Error)
            logger.Info(message);
    }

    /// <summary>
    /// 调试日志。
    /// </summary>
    /// <param name="message">调试信息。</param>
    public static void Debug(string message)
    {
        if (Level > LogLevel.Info)
            logger.Debug(message);
    }

    /// <summary>
    /// 刷新日志缓冲区。
    /// </summary>
    public static void Flush() => logger.Flush();

    /// <summary>
    /// 异常错误日志。
    /// </summary>
    /// <param name="ex">异常信息。</param>
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

    internal static void Start()
    {
        var thread = new Thread(FlushQueue) { IsBackground = true };
        thread.Start();
    }

    public void Error(string message) => errors.Enqueue(GetMessage("ERROR", message));
    public void Info(string message) => infos.Enqueue(GetMessage("INFO", message, true));
    public void Debug(string message) => debugs.Enqueue(GetMessage("DEBUG", message));
    public void Flush() => FlushLog();

    private static string GetMessage(string type, string message, bool consolePrint = false)
    {
        var text = $"{DateTime.Now:yyyy-MM-dd.HH:mm:ss.fff} {type} {message}";
        if (consolePrint)
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
        if (Config.IsClient)
            return;

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
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
        path = Path.Combine(path, type);
        path = Path.Combine(path, $"{DateTime.Now:yyyyMMdd}.log");
        var info = new FileInfo(path);
        if (!info.Directory.Exists)
            info.Directory.Create();

        var text = string.Join(Environment.NewLine, contents.ToArray());
        File.AppendAllText(path, text);
    }
}