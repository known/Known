namespace Known.WorkFlows;

/// <summary>
/// 工作流日志实体类。
/// </summary>
public class SysFlowLog : EntityBase
{
    /// <summary>
    /// 取得或设置业务ID。
    /// </summary>
    public string BizId { get; set; }

    /// <summary>
    /// 取得或设置步骤。
    /// </summary>
    public string StepName { get; set; }

    /// <summary>
    /// 取得或设置操作人。
    /// </summary>
    public string ExecuteBy { get; set; }

    /// <summary>
    /// 取得或设置操作时间。
    /// </summary>
    public DateTime ExecuteTime { get; set; }

    /// <summary>
    /// 取得或设置操作结果（通过、退回、撤回）。
    /// </summary>
    public string Result { get; set; }

    /// <summary>
    /// 取得或设置操作内容。
    /// </summary>
    public string Note { get; set; }
}