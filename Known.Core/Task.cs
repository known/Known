namespace Known;

/// <summary>
/// 后台任务基类。
/// </summary>
public class TaskBase
{
    /// <summary>
    /// 异步执行后台任务。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="task">后台任务。</param>
    /// <returns>执行结果。</returns>
    public virtual Task<Result> ExecuteAsync(Database db, TaskInfo task)
    {
        return Result.SuccessAsync("");
    }
}