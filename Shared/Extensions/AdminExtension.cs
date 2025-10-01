namespace Known.Extensions;

/// <summary>
/// 后台管理扩展类。
/// </summary>
public static class AdminExtension
{
    #region File
    internal static Task<List<AttachInfo>> GetAttachesAsync(this Database db, string[] bizIds)
    {
        return db.Query<SysFile>().ToListAsync<AttachInfo>(d => bizIds.Contains(d.BizId));
    }

    internal static Task<List<AttachInfo>> GetAttachesAsync(this Database db, string bizId)
    {
        return db.Query<SysFile>().ToListAsync<AttachInfo>(d => d.BizId == bizId);
    }

    internal static Task<List<AttachInfo>> GetAttachesAsync(this Database db, string bizId, string bizType)
    {
        return db.Query<SysFile>().ToListAsync<AttachInfo>(d => d.BizId == bizId && d.Type == bizType);
    }

    internal static Task<AttachInfo> GetAttachAsync(this Database db, string id)
    {
        return db.Query<SysFile>().FirstAsync<AttachInfo>(d => d.Id == id);
    }

    internal static Task DeleteFileAsync(this Database db, string id)
    {
        return db.DeleteAsync<SysFile>(id);
    }

    internal static async Task<AttachInfo> AddFileAsync(this Database db, AttachFile info)
    {
        var file = new SysFile
        {
            CompNo = db.User.CompNo,
            AppId = db.User.AppId,
            Category1 = info.Category1 ?? "File",
            Category2 = info.Category2,
            Type = info.BizType,
            BizId = info.BizId,
            Name = info.SourceName,
            Path = info.FilePath,
            Size = info.Size,
            SourceName = info.SourceName,
            ExtName = info.ExtName,
            ThumbPath = info.ThumbPath,
            Note = info.Note
        };
        await db.SaveAsync(file);
        return Utils.MapTo<AttachInfo>(file);
    }
    #endregion

    #region Task
    /// <summary>
    /// 异步根据业务ID获取任务信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="bizId">业务ID。</param>
    /// <returns></returns>
    internal static Task<SysTask> GetTaskAsync(this Database db, string bizId)
    {
        return db.Query<SysTask>().Where(d => d.CreateBy == db.UserName && d.BizId == bizId)
                 .OrderByDescending(d => d.CreateTime).FirstAsync();
    }
    #endregion

    #region Log
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
    #endregion
}