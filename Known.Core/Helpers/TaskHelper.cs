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
    public static void NotifyRun(TaskInfo task, Context context = null)
    {
        Task.Run(() => RunAsync(task, context));
    }

    private static Task RunAsync(TaskInfo task, Context context)
    {
        if (!Config.TaskTypes.TryGetValue(task.Type, out Type type))
            return Task.CompletedTask;

        if (Activator.CreateInstance(type) is not TaskBase handler)
            throw new InvalidOperationException("The TaskHandler is not register.");

        handler.Context = context;
        return RunAsync(task, handler.ExecuteAsync);
    }

    private static async Task RunAsync(TaskInfo task, Func<Database, TaskInfo, Task<Result>> action)
    {
        var db = Database.Create();
        try
        {
            var user = await db.GetUserAsync(task.CreateBy);
            db.User = user;

            task.BeginTime = DateTime.Now;
            task.Status = TaskJobStatus.Running;
            await db.SaveTaskAsync(task);

            var result = await action.Invoke(db, task);
            task.EndTime = DateTime.Now;
            task.Status = result.IsValid ? TaskJobStatus.Success : TaskJobStatus.Failed;
            task.Note = result.Message;
            await db.SaveTaskAsync(task);
        }
        catch (Exception ex)
        {
            task.Status = TaskJobStatus.Failed;
            task.Note = ex.ToString();
            await db.SaveTaskAsync(task);
        }
    }
}