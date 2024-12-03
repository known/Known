namespace Known.Core;

/// <summary>
/// 系统后台任务信息类。
/// </summary>
public class TaskInfo
{
    /// <summary>
    /// 构造函数，创建一个任务信息类的实例。
    /// </summary>
    public TaskInfo()
    {
        Id = Utils.GetGuid();
    }

    /// <summary>
    /// 取得或设置ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置创建时间。
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 取得或设置任务创建人。
    /// </summary>
    public string CreateBy { get; set; }

    /// <summary>
    /// 取得或设置业务ID。
    /// </summary>
    public string BizId { get; set; }

    /// <summary>
    /// 取得或设置类型。
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置执行目标。
    /// </summary>
    public string Target { get; set; }

    /// <summary>
    /// 取得或设置执行状态。
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// 取得或设置开始时间。
    /// </summary>
    public DateTime? BeginTime { get; set; }

    /// <summary>
    /// 取得或设置结束时间。
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置任务关联的附件信息。
    /// </summary>
    public AttachInfo File { get; set; }
}

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
    public const string Pending = "Pending";

    /// <summary>
    /// 执行中。
    /// </summary>
    public const string Running = "Running";

    /// <summary>
    /// 执行成功。
    /// </summary>
    public const string Success = "Success";

    /// <summary>
    /// 执行失败。
    /// </summary>
    public const string Failed = "Failed";
}