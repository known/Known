namespace Known;

public partial class Logger
{
    private static void WriteLog(LogLevel type, LogTarget target, UserInfo user, string content)
    {
        var log = new LogInfo
        {
            Type = type.ToString(),
            Target = target.ToString(),
            CreateBy = user?.Name ?? user?.UserName,
            CreateTime = DateTime.Now,
            Content = content
        };
        if (target == LogTarget.FrontEnd)
        {
            try
            {
                var scope = Config.ServiceProvider?.CreateScope();
                var service = scope?.ServiceProvider?.GetRequiredService<ILogService>();
                service?.AddWebLogAsync(log);
            }
            catch
            {
                Logs.Add(log);
            }
        }
        else
        {
            Logs.Add(log);
        }
    }
}

/// <summary>
/// 控制台日志类型枚举。
/// </summary>
public enum ConsoleLogType
{
    /// <summary>
    /// 信息。
    /// </summary>
    Info,
    /// <summary>
    /// 错误。
    /// </summary>
    Error
}

/// <summary>
/// 控制台日志信息类。
/// </summary>
public class ConsoleLogInfo
{
    /// <summary>
    /// 取得或设置业务ID。
    /// </summary>
    public string BizId { get; set; }

    /// <summary>
    /// 取得或设置日志类型。
    /// </summary>
    public ConsoleLogType Type { get; set; }

    /// <summary>
    /// 取得或设置日志内容。
    /// </summary>
    public string Content { get; set; }
}