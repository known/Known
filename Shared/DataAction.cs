namespace Known;

/// <summary>
/// 数据访问委托配置类。
/// </summary>
public class DataAction
{
    /// <summary>
    /// 取得或设置根据用户名获取用户信息的异步委托。
    /// </summary>
    public static Func<Database, string, Task<UserInfo>> OnGetUser { get; set; }

    /// <summary>
    /// 取得或设置根据业务ID获取任务信息的异步委托。
    /// </summary>
    public static Func<Database, string, Task<TaskInfo>> OnGetTask { get; set; }

    /// <summary>
    /// 取得或设置创建任务信息的异步委托。
    /// </summary>
    public static Func<Database, TaskInfo, Task> OnCreateTask { get; set; }

    /// <summary>
    /// 取得或设置保存任务信息的异步委托。
    /// </summary>
    public static Func<Database, TaskInfo, Task> OnSaveTask { get; set; }
}