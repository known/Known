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
    /// <param name="info">日志信息。</param>
    /// <returns></returns>
    public static async Task<Result> AddLogAsync(this Database db, LogInfo info)
    {
        //if (info.Type == LogType.Page &&
        //    string.IsNullOrWhiteSpace(info.Target) &&
        //    !string.IsNullOrWhiteSpace(info.Content))
        //{
        //    var module = info.Content.StartsWith("/page/")
        //               ? AppData.GetModule(info.Content.Substring(6))
        //               : AppData.GetModule(d => d.Url == info.Content);
        //    info.Target = module?.Name;
        //}

        await db.SaveAsync(new SysLog
        {
            Type = info.Type.ToString(),
            Target = info.Target,
            Content = info.Content
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