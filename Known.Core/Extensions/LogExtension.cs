namespace Known.Extensions;

/// <summary>
/// 日志数据扩展类。
/// </summary>
public static class LogExtension
{
    /// <summary>
    /// 异步添加操作日志。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="log">日志信息。</param>
    /// <returns></returns>
    public static async Task<Result> AddLogAsync(this Database db, LogInfo log)
    {
        //if (log.Type == LogType.Page &&
        //    string.IsNullOrWhiteSpace(log.Target) &&
        //    !string.IsNullOrWhiteSpace(log.Content))
        //{
        //    var module = log.Content.StartsWith("/page/")
        //               ? AppData.GetModule(log.Content.Substring(6))
        //               : AppData.GetModule(d => d.Url == log.Content);
        //    log.Target = module?.Name;
        //}

        await db.SaveAsync(new SysLog
        {
            Type = log.Type.ToString(),
            Target = log.Target,
            Content = log.Content
        });
        return Result.Success("");
    }

    /// <summary>
    /// 异步添加操作日志。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="type">日志类型。</param>
    /// <param name="target">操作对象。</param>
    /// <param name="content">操作内容。</param>
    /// <returns></returns>
    public static Task AddLogAsync(this Database db, LogType type, string target, string content)
    {
        return db.SaveAsync(new SysLog
        {
            Type = type.ToString(),
            Target = target,
            Content = content
        });
    }
}