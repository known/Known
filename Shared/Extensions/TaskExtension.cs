namespace Known.Extensions;

/// <summary>
/// 数据访问扩展类。
/// </summary>
public static class TaskExtension
{
    /// <summary>
    /// 取得或设置根据业务ID获取任务信息的异步委托。
    /// </summary>
    public static Func<Database, string, Task<TaskInfo>> OnGetTask { get; set; } = (db, bizId) => Task.FromResult(default(TaskInfo));

    /// <summary>
    /// 取得或设置创建任务信息的异步委托。
    /// </summary>
    public static Func<Database, TaskInfo, Task> OnCreateTask { get; set; } = (db, info) => Task.CompletedTask;

    /// <summary>
    /// 取得或设置保存任务信息的异步委托。
    /// </summary>
    public static Func<Database, TaskInfo, Task> OnSaveTask { get; set; } = (db, info) => Task.CompletedTask;

    /// <summary>
    /// 异步根据业务ID获取任务信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="bizId">业务ID。</param>
    /// <returns></returns>
    public static Task<TaskInfo> GetTaskAsync(this Database db, string bizId) => OnGetTask(db, bizId);

    /// <summary>
    /// 异步创建一个后台任务。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">任务信息。</param>
    /// <returns></returns>
    public static Task CreateTaskAsync(this Database db, TaskInfo info) => OnCreateTask.Invoke(db, info);

    internal static Task SaveTaskAsync(this Database db, TaskInfo info) => OnSaveTask.Invoke(db, info);
}