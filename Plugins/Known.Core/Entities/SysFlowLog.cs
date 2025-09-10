namespace Known.Entities;

/// <summary>
/// 工作流日志实体类。
/// </summary>
[DisplayName("工作流日志")]
public class SysFlowLog : EntityBase
{
    /// <summary>
    /// 取得或设置业务ID。
    /// </summary>
    [DisplayName("业务ID")]
    public string BizId { get; set; }

    /// <summary>
    /// 取得或设置步骤。
    /// </summary>
    [DisplayName("步骤")]
    public string StepName { get; set; }

    /// <summary>
    /// 取得或设置操作人。
    /// </summary>
    [DisplayName("操作人")]
    public string ExecuteBy { get; set; }

    /// <summary>
    /// 取得或设置操作时间。
    /// </summary>
    [DisplayName("操作时间")]
    public DateTime ExecuteTime { get; set; }

    /// <summary>
    /// 取得或设置操作结果（通过、退回、撤回）。
    /// </summary>
    [DisplayName("操作结果")]
    public string Result { get; set; }

    /// <summary>
    /// 取得或设置操作内容。
    /// </summary>
    [DisplayName("操作内容")]
    public string Note { get; set; }
}