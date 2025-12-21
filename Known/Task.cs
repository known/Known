namespace Known;

/// <summary>
/// 系统定时任务状态类，代码表，类别是类名称。
/// </summary>
[CodeInfo]
public class TaskJobStatus
{
    private TaskJobStatus() { }

    /// <summary>
    /// 待执行。
    /// </summary>
    public const string Pending = "待执行";

    /// <summary>
    /// 执行中。
    /// </summary>
    public const string Running = "执行中";

    /// <summary>
    /// 执行成功。
    /// </summary>
    public const string Success = "执行成功";

    /// <summary>
    /// 执行失败。
    /// </summary>
    public const string Failed = "执行失败";
}

/// <summary>
/// 后台任务基类。
/// </summary>
public class TaskBase
{
    /// <summary>
    /// 取得或设置系统上下文。
    /// </summary>
    public Context Context { get; set; }

    /// <summary>
    /// 异步执行后台任务。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="task">后台任务。</param>
    /// <returns>执行结果。</returns>
    public virtual Task<Result> ExecuteAsync(Database db, SysTask task)
    {
        return Result.SuccessAsync("");
    }
}

[Task(ImportHelper.BizType)]
class ImportTask(INotifyService notify) : TaskBase
{
    public override async Task<Result> ExecuteAsync(Database db, SysTask task)
    {
        var result = await ImportHelper.ExecuteAsync(Context, db, task);
        await notify.LayoutNotifyAsync("任务执行通知", $"任务[{task.Name}]执行完成。{Environment.NewLine}结果：{result.Message}");
        return result;
    }
}