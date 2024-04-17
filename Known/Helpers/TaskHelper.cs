using System.Globalization;
using Known.Entities;
using Known.Repositories;

namespace Known.Helpers;

sealed class TaskHelper
{
    private TaskHelper() { }

    private static async Task<TaskSummaryInfo> GetSummaryAsync(Database db, string type)
    {
        var task = await SystemRepository.GetTaskByTypeAsync(db, type);
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
        var task = await SystemRepository.GetTaskByTypeAsync(db, type);
        if (task != null)
        {
            switch (task.Status)
            {
                case TaskStatus.Pending:
                    return Result.Success(db.Context?.Language["Tip.TaskPending"]);
                case TaskStatus.Running:
                    return Result.Success(db.Context?.Language["Tip.TaskRunning"]);
            }
        }

        await db.SaveAsync(new SysTask
        {
            BizId = type,
            Type = type,
            Name = name,
            Target = target,
            Status = TaskStatus.Pending
        });
        return Result.Success(db.Context?.Language["Tip.TaskAddSuccess"]);
    }

    internal static async Task RunAsync(string bizType, Func<Database, SysTask, Task<Result>> action)
    {
        var db = new Database { Context = new Context(CultureInfo.CurrentCulture.Name) };
        var task = await SystemRepository.GetPendingTaskByTypeAsync(db, bizType);
        if (task == null)
            return;

        await RunAsync(db, task, action);
    }

    internal static async Task<Result> RunAsync(Database db, SysTask task, Func<Database, SysTask, Task<Result>> action)
    {
        var userName = task.CreateBy;
        db.User = await UserRepository.GetUserInfoAsync(db, userName);

        task.BeginTime = DateTime.Now;
        task.Status = TaskStatus.Running;
        await db.SaveAsync(task);

        var result = await action.Invoke(db, task);
        task.EndTime = DateTime.Now;
        task.Status = result.IsValid ? TaskStatus.Success : TaskStatus.Failed;
        task.Note = result.Message;
        await db.SaveAsync(task);
        return result;
    }
}