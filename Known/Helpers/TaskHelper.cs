namespace Known.Helpers;

/// <summary>
/// 后台异步任务帮助者类。
/// </summary>
public sealed class TaskHelper
{
    private TaskHelper() { }

    /// <summary>
    /// 通知运行指定业务类型的后台任务。
    /// </summary>
    /// <param name="task">业务类型。</param>
    /// <param name="context">系统上下文。</param>
    public static void NotifyRun(SysTask task, Context context = null)
    {
        Task.Run(() => RunAsync(task, context));
    }

    private static Task RunAsync(SysTask task, Context context)
    {
        if (!CoreConfig.TaskTypes.TryGetValue(task.Type, out Type type))
            return Task.CompletedTask;

        if (Config.CreateService(type) is not TaskBase handler)
            throw new InvalidOperationException($"The {task.Type} is not register.");

        handler.Context = context;
        return RunAsync(task, handler.ExecuteAsync);
    }

    private static async Task RunAsync(SysTask task, Func<Database, SysTask, Task<Result>> action)
    {
        var notify = Config.CreateService<INotifyService>();
        using var db = Database.Create();
        try
        {
            var user = await db.GetUserAsync(task.CreateBy);
            db.User = user;

            task.BeginTime = DateTime.Now;
            task.Status = TaskJobStatus.Running;
            await db.SaveAsync(task);

            var result = await action.Invoke(db, task);
            task.EndTime = DateTime.Now;
            task.Status = result.IsValid ? TaskJobStatus.Success : TaskJobStatus.Failed;
            task.Note = result.Message;
            await db.SaveAsync(task);
            await notify.LayoutNotifyAsync(task.CreateBy, Language.TaskNotify, $"{task.Status}：{task.Note}");
        }
        catch (Exception ex)
        {
            task.Status = TaskJobStatus.Failed;
            task.Note = ex.ToString();
            await db.SaveAsync(task);
            await notify.LayoutNotifyAsync(task.CreateBy, Language.TaskNotify, $"执行错误：{ex.Message}", StyleType.Error);
        }
    }
}