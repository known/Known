namespace Known.Extensions;

static class DataExtension
{
    internal static async Task CreateTaskAsync(this Database db, TaskInfo info)
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