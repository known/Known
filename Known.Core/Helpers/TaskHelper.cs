namespace Known.Core.Helpers;

public class TaskHelper
{
    private TaskHelper() { }

    public static SysTask GetByType(Database db, string type)
    {
        return TaskRepository.GetTaskByType(db, type);
    }

    public static TaskSummaryInfo GetTaskSummary(Database db, string type)
    {
        var task = TaskRepository.GetTaskByType(db, type);
        if (task == null)
            return null;

        var span = task.EndTime - task.BeginTime;
        var time = span.HasValue ? $"{span.Value.TotalMilliseconds}" : "";
        return new TaskSummaryInfo
        {
            Status = task.Status,
            Message = $"执行时间：{task.CreateTime:yyyy-MM-dd HH:mm:ss}，耗时：{time}毫秒"
        };
    }

    public static Result AddTask(Database db, string type, string name, string target = "")
    {
        var task = TaskRepository.GetTaskByType(db, type);
        if (task != null)
        {
            switch (task.Status)
            {
                case Constants.TaskPending:
                    return Result.Success("任务等待中...");
                case Constants.TaskRunning:
                    return Result.Success("任务执行中...");
            }
        }

        db.Save(new SysTask
        {
            BizId = type,
            Type = type,
            Name = name,
            Target = target,
            Status = Constants.TaskPending
        });
        return Result.Success("任务添加成功，请稍后查询结果！");
    }

    public static void Run(string bizType, Func<Database, SysTask, Result> action)
    {
        var db = new Database();
        var task = TaskRepository.GetPendingTaskByType(db, bizType);
        if (task == null)
            return;

        var userName = task.CreateBy.Split('-')[0];
        db.User = UserRepository.GetUser(db, userName);

        task.BeginTime = DateTime.Now;
        task.Status = Constants.TaskRunning;
        db.Save(task);

        var result = action.Invoke(db, task);
        task.EndTime = DateTime.Now;
        task.Status = result.IsValid ? Constants.TaskSuccess : Constants.TaskFailed;
        task.Note = result.Message;
        db.Save(task);
    }
}