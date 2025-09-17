namespace Known.Extensions;

/// <summary>
/// 后台管理扩展类。
/// </summary>
public static class AdminExtension
{
    /// <summary>
    /// 取得或设置后台管理服务接口实例。
    /// </summary>
    public static IAdminPService Service { get; set; }

    #region Task
    /// <summary>
    /// 异步根据业务ID获取任务信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="bizId">业务ID。</param>
    /// <returns></returns>
    public static Task<TaskInfo> GetTaskAsync(this Database db, string bizId) => Service?.GetTaskAsync(db, bizId);

    /// <summary>
    /// 异步创建一个后台任务。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">任务信息。</param>
    /// <returns></returns>
    public static Task CreateTaskAsync(this Database db, TaskInfo info) => Service?.CreateTaskAsync(db, info);

    internal static Task SaveTaskAsync(this Database db, TaskInfo info) => Service?.SaveTaskAsync(db, info);
    #endregion

    #region Log
    /// <summary>
    /// 异步获取常用功能菜单信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="userName">用户名。</param>
    /// <param name="size">Top数量。</param>
    /// <returns>功能菜单信息。</returns>
    public static Task<List<string>> GetVisitMenuIdsAsync(this Database db, string userName, int size)
    {
        return Service?.GetVisitMenuIdsAsync(db, userName, size);
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

        await Service?.SaveLogAsync(db, info);
        return Result.Success("");
    }
    #endregion
}