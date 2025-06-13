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
    /// 通知运行指定业务类型的后台任务。
    /// </summary>
    /// <param name="bizType">业务类型。</param>
    public static void NotifyRun(string bizType)
    {
        RunSwitches[bizType] = true;
        if (bizType == ImportHelper.BizType)
            Task.Run(ImportHelper.ExecuteAsync);
        else if (bizType == WeixinHelper.BizType)
            Task.Run(WeixinHelper.ExecuteAsync);
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
        db.Context = new Context();

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
            var info = await db.Query<SysTask>().Where(d => d.Status == TaskJobStatus.Pending && d.Type == bizType)
                               .OrderBy(d => d.CreateTime).FirstAsync<TaskInfo>();
            if (info != null)
            {
                db.User = await db.GetUserAsync(info.CreateBy);
                info.File = await db.Query<SysFile>().Where(d => d.Id == info.Target).FirstAsync<AttachInfo>();
            }
            return info;
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
            task.Status = TaskJobStatus.Running;
            await db.SaveTaskAsync(task);

            var result = await action.Invoke(db, task);
            task.EndTime = DateTime.Now;
            task.Status = result.IsValid ? TaskJobStatus.Success : TaskJobStatus.Failed;
            task.Note = result.Message;
            await db.SaveTaskAsync(task);
            return result;
        }
        catch (Exception ex)
        {
            task.Status = TaskJobStatus.Failed;
            task.Note = ex.ToString();
            await db.SaveTaskAsync(task);
            return Result.Error(ex.Message);
        }
    }
}