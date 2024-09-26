namespace Known.Helpers;

sealed class TaskHelper
{
    private static readonly Dictionary<string, bool> RunStates = [];
    private static readonly IDataRepository Repository = Database.CreateRepository();

    private TaskHelper() { }

    internal static async Task RunAsync(string bizType, Func<Database, SysTask, Task<Result>> action)
    {
        if (RunStates.TryGetValue(bizType, out bool value) && value)
            return;

        RunStates[bizType] = true;
        var db = Database.Create();
        db.Context = new Context(CultureInfo.CurrentCulture.Name);

        try
        {
            var task = await Repository.GetPendingTaskAsync(db, bizType);
            if (task == null)
            {
                RunStates[bizType] = false;
                return;
            }

            await RunAsync(db, task, action);
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
        }
        finally
        {
            RunStates[bizType] = false;
        }
    }

    private static async Task<Result> RunAsync(Database db, SysTask task, Func<Database, SysTask, Task<Result>> action)
    {
        var userName = task.CreateBy;
        db.User = await Platform.GetUserAsync(db, userName);
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

    private static async Task<TaskSummaryInfo> GetSummaryAsync(Database db, string type)
    {
        var task = await Repository.GetTaskByTypeAsync(db, type);
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

    private static async Task<Result> AddAsync(Database db, string type, string name, string target = "")
    {
        var task = await Repository.GetTaskByTypeAsync(db, type);
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