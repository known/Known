namespace Known;

/// <summary>
/// 日志级别枚举。
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// 跟踪。
    /// </summary>
    Trace,
    /// <summary>
    /// 调试。
    /// </summary>
    Debug,
    /// <summary>
    /// 信息。
    /// </summary>
    Information,
    /// <summary>
    /// 警告。
    /// </summary>
    Warning,
    /// <summary>
    /// 错误。
    /// </summary>
    Error,
    /// <summary>
    /// 严重。
    /// </summary>
    Critical
}

/// <summary>
/// 日志操作类。
/// </summary>
public sealed class Logger
{
    private Logger() { }

    /// <summary>
    /// 取得日志信息列表。
    /// </summary>
    public static List<LogInfo> Logs { get; } = [];

    /// <summary>
    /// 取得或设置日志级别，默认Error。
    /// </summary>
    public static LogLevel Level { get; set; } = LogLevel.Error;

    /// <summary>
    /// 初始化日志，添加日志过期自动删除服务。
    /// </summary>
    /// <param name="days">保留天数。</param>
    public static void Initialize(int days)
    {
        Task.Run(() =>
        {
            while (true)
            {
                Logs.RemoveAll(l => l.CreateTime <= DateTime.Now.AddDays(-days));
                Thread.Sleep(1000 * 60);
            }
        });
    }

    /// <summary>
    /// 异步查询日志。
    /// </summary>
    /// <param name="criteria">查询条件。</param>
    /// <returns>查询结果。</returns>
    public static Task<PagingResult<LogInfo>> QueryLogsAsync(PagingCriteria criteria)
    {
        var logs = Logger.Logs;
        var type = criteria.GetQueryValue(nameof(LogInfo.Type));
        if (!string.IsNullOrWhiteSpace(type))
            logs = logs.Where(l => l.Type == type).ToList();
        var createBy = criteria.GetQueryValue(nameof(LogInfo.CreateBy));
        if (!string.IsNullOrWhiteSpace(createBy))
            logs = logs.Where(l => l.CreateBy == createBy).ToList();
        var content = criteria.GetQueryValue(nameof(LogInfo.Content));
        if (!string.IsNullOrWhiteSpace(content))
            logs = logs.Where(l => l.Content.Contains(content)).ToList();

        logs = logs.OrderByDescending(l => l.CreateTime).ToList();
        var result = logs.ToPagingResult(criteria);
        return Task.FromResult(result);
    }

    /// <summary>
    /// 异步删除日志。
    /// </summary>
    /// <param name="infos">日志列表。</param>
    /// <returns>删除结果。</returns>
    public static Task<Result> DeleteLogsAsync(List<LogInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.ErrorAsync("请至少选择一条记录！");

        foreach (var info in infos)
        {
            Logs.Remove(info);
        }
        return Result.SuccessAsync("删除成功！");
    }

    /// <summary>
    /// 异步清理全部日志。
    /// </summary>
    /// <returns>清空结果。</returns>
    public static Task<Result> ClearLogsAsync()
    {
        Logs.Clear();
        return Result.SuccessAsync("清空成功！");
    }

    /// <summary>
    /// 添加跟踪日志。
    /// </summary>
    /// <param name="target">目标。</param>
    /// <param name="user">创建人。</param>
    /// <param name="content">日志内容。</param>
    public static void Trace(LogTarget target, UserInfo user, string content)
    {
        WriteLog(LogLevel.Trace, target, user, content);
    }

    /// <summary>
    /// 添加调试日志。
    /// </summary>
    /// <param name="target">目标。</param>
    /// <param name="user">创建人。</param>
    /// <param name="content">日志内容。</param>
    public static void Debug(LogTarget target, UserInfo user, string content)
    {
        WriteLog(LogLevel.Debug, target, user, content);
    }

    /// <summary>
    /// 添加信息日志。
    /// </summary>
    /// <param name="target">目标。</param>
    /// <param name="user">创建人。</param>
    /// <param name="content">日志内容。</param>
    public static void Information(LogTarget target, UserInfo user, string content)
    {
        WriteLog(LogLevel.Information, target, user, content);
    }

    /// <summary>
    /// 添加警告日志。
    /// </summary>
    /// <param name="target">目标。</param>
    /// <param name="user">创建人。</param>
    /// <param name="content">日志内容。</param>
    public static void Warning(LogTarget target, UserInfo user, string content)
    {
        WriteLog(LogLevel.Warning, target, user, content);
    }

    /// <summary>
    /// 添加错误日志。
    /// </summary>
    /// <param name="target">目标。</param>
    /// <param name="user">创建人。</param>
    /// <param name="content">日志内容。</param>
    public static void Error(LogTarget target, UserInfo user, string content)
    {
        WriteLog(LogLevel.Error, target, user, content);
    }

    /// <summary>
    /// 添加严重日志。
    /// </summary>
    /// <param name="target">目标。</param>
    /// <param name="user">创建人。</param>
    /// <param name="content">日志内容。</param>
    public static void Critical(LogTarget target, UserInfo user, string content)
    {
        WriteLog(LogLevel.Critical, target, user, content);
    }

    private static void WriteLog(LogLevel type, LogTarget target, UserInfo user, string content)
    {
        if (Level > type)
            return;

        var log = new LogInfo { Type = type.ToString(), Target = target.ToString(), CreateBy = user?.UserName, CreateTime = DateTime.Now, Content = content };
        Logs.Add(log);
    }
}