namespace Known.Helpers;

/// <summary>
/// 后台异步任务帮助者类。
/// </summary>
public sealed class TaskHelper
{
    private static readonly Dictionary<string, bool> RunSwitches = [];
    private static readonly Dictionary<string, bool> RunStates = [];

    private TaskHelper() { }

    /// <summary>
    /// 取得或设置获取等待执行的后台任务委托。
    /// </summary>
    public static Func<Database, string, Task<TaskInfo>> OnPendingTask { get; set; }

    /// <summary>
    /// 取得或设置保存后台任务状态委托。
    /// </summary>
    public static Func<Database, TaskInfo, Task> OnSaveTask { get; set; }

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
    public static async Task RunAsync(string bizType, Func<Database, TaskInfo, Task<Result>> action)
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

    private static async Task<TaskInfo> GetPendingTaskAsync(Database db, string bizType)
    {
        try
        {
            return await OnPendingTask?.Invoke(db, bizType);
        }
        catch
        {
            return null;
        }
    }

    private static async Task<Result> RunAsync(Database db, TaskInfo task, Func<Database, TaskInfo, Task<Result>> action)
    {
        try
        {
            task.BeginTime = DateTime.Now;
            task.Status = Core.TaskJobStatus.Running;
            await OnSaveTask?.Invoke(db, task);

            var result = await action.Invoke(db, task);
            task.EndTime = DateTime.Now;
            task.Status = result.IsValid ? Core.TaskJobStatus.Success : Core.TaskJobStatus.Failed;
            task.Note = result.Message;
            await OnSaveTask?.Invoke(db, task);
            return result;
        }
        catch (Exception ex)
        {
            task.Status = Core.TaskJobStatus.Failed;
            task.Note = ex.ToString();
            await OnSaveTask?.Invoke(db, task);
            return Result.Error(ex.Message);
        }
    }
}