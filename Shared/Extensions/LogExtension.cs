namespace Known.Extensions;

/// <summary>
/// 日志数据扩展类。
/// </summary>
public static class LogExtension
{
    /// <summary>
    /// 异步获取常用功能菜单信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="userName">用户名。</param>
    /// <param name="size">Top数量。</param>
    /// <returns>功能菜单信息。</returns>
    public static async Task<List<string>> GetVisitMenuIdsAsync(this Database db, string userName, int size)
    {
        var logs = await db.Query<SysLog>()
                           .Where(d => d.Type == $"{LogType.Page}" && d.CreateBy == userName)
                           .GroupBy(d => d.Target)
                           .Select(d => new CountInfo { Field1 = d.Target, TotalCount = DbFunc.Count() })
                           .ToListAsync();
        logs = logs?.OrderByDescending(f => f.TotalCount).Take(size).ToList();
        return logs?.Select(l => l.Field1).ToList();
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
        return db.AddLogAsync(new LogInfo
        {
            Type = type.ToString(),
            Target = target,
            Content = content
        });
    }

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

        if (!Config.IsAdminLog && db.User.IsSystemAdmin() && info.Type != nameof(LogType.Register))
            return Result.Success("");

        await db.SaveAsync(new SysLog
        {
            Type = info.Type.ToString(),
            Target = info.Target ?? "",
            Content = info.Content
        });
        return Result.Success("");
    }
}