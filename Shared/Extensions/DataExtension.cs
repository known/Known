namespace Known.Extensions;

/// <summary>
/// 数据访问扩展类。
/// </summary>
public static class DataExtension
{
    /// <summary>
    /// 异步根据用户名获取用户信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="userName">用户名。</param>
    /// <returns>用户信息。</returns>
    public static Task<UserInfo> GetUserAsync(this Database db, string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return Task.FromResult(default(UserInfo));

        if (DataAction.OnGetUser == null)
            return Task.FromResult(default(UserInfo));

        userName = userName.ToLower();
        return DataAction.OnGetUser.Invoke(db, userName);
    }

    /// <summary>
    /// 异步根据业务ID获取任务信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="bizId">业务ID。</param>
    /// <returns></returns>
    public static Task<TaskInfo> GetTaskAsync(this Database db, string bizId)
    {
        if (DataAction.OnGetTask == null)
            return Task.FromResult(default(TaskInfo));

        return DataAction.OnGetTask.Invoke(db, bizId);
    }

    /// <summary>
    /// 异步创建一个后台任务。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">任务信息。</param>
    /// <returns></returns>
    public static Task CreateTaskAsync(this Database db, TaskInfo info)
    {
        if (DataAction.OnCreateTask == null)
            return Task.CompletedTask;

        return DataAction.OnCreateTask.Invoke(db, info);
    }

    internal static Task SaveTaskAsync(this Database db, TaskInfo info)
    {
        if (DataAction.OnSaveTask == null)
            return Task.CompletedTask;

        return DataAction.OnSaveTask.Invoke(db, info);
    }
}