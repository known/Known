namespace Known.Extensions;

static class LogExtension
{
    internal static async Task<Result> AddLogAsync(this Database db, LogInfo log)
    {
        if (log.Type == LogType.Page &&
            string.IsNullOrWhiteSpace(log.Target) &&
            !string.IsNullOrWhiteSpace(log.Content))
        {
            var module = log.Content.StartsWith("/page/")
                       ? await db.QueryByIdAsync<SysModule>(log.Content.Substring(6))
                       : await db.QueryAsync<SysModule>(d => d.Url == log.Content);
            log.Target = module?.Name;
        }

        await db.SaveAsync(new SysLog
        {
            Type = log.Type.ToString(),
            Target = log.Target,
            Content = log.Content
        });
        return Result.Success("");
    }

    internal static Task AddLogAsync(this Database db, LogType type, string target, string content)
    {
        return db.SaveAsync(new SysLog
        {
            Type = type.ToString(),
            Target = target,
            Content = content
        });
    }
}