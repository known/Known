namespace Known.Extensions;

/// <summary>
/// 日志提供者扩展类。
/// </summary>
public static class LoggerExtension
{
    /// <summary>
    /// 记录调试类日志。
    /// </summary>
    /// <param name="logger">日志提供者。</param>
    /// <param name="debug">调试日志。</param>
    public static void Debug(this ILogger logger, object debug)
    {
        logger.LogDebug($"{DateTime.Now:yyy-MM-dd HH:mm:ss} => {debug}");
    }

    /// <summary>
    /// 记录信息类日志。
    /// </summary>
    /// <param name="logger">日志提供者。</param>
    /// <param name="info">信息日志。</param>
    public static void Info(this ILogger logger, object info)
    {
        logger.LogInformation($"{DateTime.Now:yyy-MM-dd HH:mm:ss} => {info}");
    }

    /// <summary>
    /// 记录错误类日志。
    /// </summary>
    /// <param name="logger">日志提供者。</param>
    /// <param name="error">错误日志。</param>
    public static void Error(this ILogger logger, object error)
    {
        logger.LogError($"{DateTime.Now:yyy-MM-dd HH:mm:ss} => {error}");
    }
}