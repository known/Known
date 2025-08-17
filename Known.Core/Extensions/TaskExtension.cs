namespace Known.Extensions;

/// <summary>
/// 后台任务扩展类。
/// </summary>
public static class TaskExtension
{
    internal static Task<TaskInfo> GetTaskAsync(this Database db, string bizId)
    {
        return db.Query<SysTask>().Where(d => d.CreateBy == db.UserName && d.BizId == bizId)
                 .OrderByDescending(d => d.CreateTime).FirstAsync<TaskInfo>();
    }

    /// <summary>
    /// 异步创建一个后台任务。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">任务信息。</param>
    /// <returns></returns>
    public static async Task CreateTaskAsync(this Database db, TaskInfo info)
    {
        var task = new SysTask();
        if (!string.IsNullOrWhiteSpace(info.Id))
            task.Id = info.Id;
        task.BizId = info.BizId;
        task.Type = info.Type;
        task.Name = info.Name;
        task.Target = info.Target;
        task.Status = info.Status;
        await db.SaveAsync(task);
    }

    internal static async Task SaveTaskAsync(this Database db, TaskInfo info)
    {
        var task = await db.QueryByIdAsync<SysTask>(info.Id);
        if (task == null)
            return;

        task.Status = info.Status;
        task.BeginTime = info.BeginTime;
        task.EndTime = info.EndTime;
        task.Note = info.Note;
        await db.SaveAsync(task);
    }
}