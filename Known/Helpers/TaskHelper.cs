namespace Known.Helpers;

sealed class TaskHelper
{
    private TaskHelper() { }

    internal static async Task RunAsync(string bizType, Func<Database, SysTask, Task<Result>> action)
    {
        var db = Platform.CreateDatabase();
        db.Context = new Context(CultureInfo.CurrentCulture.Name);
        var repository = Platform.CreateRepository();
        var task = await repository.GetPendingTaskAsync(db, bizType);
        if (task == null)
            return;

        await RunAsync(db, task, action);
    }

    private static async Task<Result> RunAsync(Database db, SysTask task, Func<Database, SysTask, Task<Result>> action)
    {
        var userName = task.CreateBy;
        db.User = await db.QueryAsync<UserInfo>(d => d.UserName == userName);

        task.BeginTime = DateTime.Now;
        task.Status = SysTaskStatus.Running;
        await db.SaveAsync(task);

        var result = await action.Invoke(db, task);
        task.EndTime = DateTime.Now;
        task.Status = result.IsValid ? SysTaskStatus.Success : SysTaskStatus.Failed;
        task.Note = result.Message;
        await db.SaveAsync(task);
        return result;
    }

    private static async Task<TaskSummaryInfo> GetSummaryAsync(Database db, IDataRepository repository, string type)
    {
        var task = await repository.GetTaskByTypeAsync(db, type);
        if (task == null)
            return null;

        var span = task.EndTime - task.BeginTime;
        var time = span.HasValue ? $"{span.Value.TotalMilliseconds}" : "";
        return new TaskSummaryInfo
        {
            Status = task.Status,
            Message = db.Context?.Language["Tip.TaskInfo"].Replace("{createTime}", $"{task.CreateTime:yyyy-MM-dd HH:mm:ss}").Replace("{time}", time)
        };
    }

    private static async Task<Result> AddAsync(Database db, IDataRepository repository, string type, string name, string target = "")
    {
        var task = await repository.GetTaskByTypeAsync(db, type);
        if (task != null)
        {
            switch (task.Status)
            {
                case SysTaskStatus.Pending:
                    return Result.Success(db.Context?.Language["Tip.TaskPending"]);
                case SysTaskStatus.Running:
                    return Result.Success(db.Context?.Language["Tip.TaskRunning"]);
            }
        }

        await db.SaveAsync(new SysTask
        {
            BizId = type,
            Type = type,
            Name = name,
            Target = target,
            Status = SysTaskStatus.Pending
        });
        return Result.Success(db.Context?.Language["Tip.TaskAddSuccess"]);
    }
}