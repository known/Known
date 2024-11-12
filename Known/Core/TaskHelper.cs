namespace Known.Core;

/// <summary>
/// 后台异步任务帮助者类。
/// </summary>
public sealed class TaskHelper
{
    private static readonly Dictionary<string, bool> RunSwitches = [];
    private static readonly Dictionary<string, bool> RunStates = [];

    private TaskHelper() { }

    /// <summary>
    /// 通知运行指定业务类型的后台任务。
    /// </summary>
    /// <param name="bizType">业务类型。</param>
    public static void NotifyRun(string bizType)
    {
        RunSwitches[bizType] = true;
    }

    /// <summary>
    /// 异步运行指定业务类型的后台任务。
    /// </summary>
    /// <param name="bizType">业务类型。</param>
    /// <param name="action">后台任务执行委托。</param>
    /// <returns></returns>
    public static async Task RunAsync(string bizType, Func<Database, SysTask, Task<Result>> action)
    {
        if (RunSwitches.TryGetValue(bizType, out bool enabled) && !enabled)
            return;

        if (RunStates.TryGetValue(bizType, out bool value) && value)
            return;

        RunStates[bizType] = true;
        var db = Database.Create();
        db.Context = new Context(CultureInfo.CurrentCulture.Name);

        db.EnableLog = false;
        var task = await GetPendingTaskAsync(db, bizType);
        if (task == null || string.IsNullOrWhiteSpace(task.BizId))
        {
            RunSwitches[bizType] = false;
            RunStates[bizType] = false;
            return;
        }

        db.EnableLog = true;
        await RunAsync(db, task, action);
        RunStates[bizType] = false;
    }

    private static async Task<SysTask> GetPendingTaskAsync(Database db, string bizType)
    {
        try
        {
            var task = await db.Query<SysTask>().Where(d => d.Status == SysTaskStatus.Pending && d.Type == bizType)
                               .OrderBy(d => d.CreateTime).FirstAsync();
            return task;
        }
        catch
        {
            return null;
        }
    }

    private static async Task<Result> RunAsync(Database db, SysTask task, Func<Database, SysTask, Task<Result>> action)
    {
        try
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
        catch (Exception ex)
        {
            task.Status = SysTaskStatus.Failed;
            task.Note = ex.ToString();
            await db.SaveAsync(task);
            return Result.Error(ex.Message);
        }
    }

    private static async Task<TaskSummaryInfo> GetSummaryAsync(Database db, string type)
    {
        var task = await GetTaskByTypeAsync(db, type);
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
        var task = await GetTaskByTypeAsync(db, type);
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

    private static Task<SysTask> GetTaskByTypeAsync(Database db, string type)
    {
        return db.Query<SysTask>().Where(d => d.Type == type)
                 .OrderByDescending(d => d.CreateTime).FirstAsync();
    }
}