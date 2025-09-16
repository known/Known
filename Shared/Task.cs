namespace Known;

/// <summary>
/// 系统后台任务信息类。
/// </summary>
[DisplayName("后台任务")]
public class TaskInfo
{
    /// <summary>
    /// 构造函数，创建一个任务信息类的实例。
    /// </summary>
    public TaskInfo()
    {
        Id = Utils.GetNextId();
    }

    /// <summary>
    /// 取得或设置ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置任务创建人。
    /// </summary>
    public string CreateBy { get; set; }

    /// <summary>
    /// 取得或设置创建时间。
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 取得或设置业务ID。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [DisplayName("业务ID")]
    public string BizId { get; set; }

    /// <summary>
    /// 取得或设置类型。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column(IsQuery = true)]
    [DisplayName("类型")]
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column(IsQuery = true)]
    [DisplayName("名称")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置执行目标。
    /// </summary>
    [Column]
    [DisplayName("目标")]
    public string Target { get; set; }

    /// <summary>
    /// 取得或设置执行状态。
    /// </summary>
    [Category(nameof(TaskJobStatus))]
    [Required]
    [MaxLength(50)]
    [Column]
    [DisplayName("状态")]
    public string Status { get; set; }

    /// <summary>
    /// 取得或设置开始时间。
    /// </summary>
    [Column]
    [DisplayName("开始时间")]
    public DateTime? BeginTime { get; set; }

    /// <summary>
    /// 取得或设置结束时间。
    /// </summary>
    [Column]
    [DisplayName("结束时间")]
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column]
    [DisplayName("备注")]
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
    public virtual Task<Result> ExecuteAsync(Database db, TaskInfo task)
    {
        return Result.SuccessAsync("");
    }
}

[Task(ImportHelper.BizType)]
class ImportTask : TaskBase
{
    public override Task<Result> ExecuteAsync(Database db, TaskInfo task)
    {
        return ImportHelper.ExecuteAsync(Context, db, task);
    }
}