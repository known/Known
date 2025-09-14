namespace Known.Extensions;

static class TaskExtension
{
    internal static Task<TaskInfo> GetSysTaskAsync(this Database db, string bizId)
    {
        return db.Query<SysTask>().Where(d => d.CreateBy == db.UserName && d.BizId == bizId)
                 .OrderByDescending(d => d.CreateTime).FirstAsync<TaskInfo>();
    }

    internal static async Task CreateSysTaskAsync(this Database db, TaskInfo info)
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

    internal static async Task SaveSysTaskAsync(this Database db, TaskInfo info)
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