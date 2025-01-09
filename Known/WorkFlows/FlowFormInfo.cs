namespace Known.WorkFlows;

/// <summary>
/// 工作流表单信息类。
/// </summary>
public class FlowFormInfo
{
    /// <summary>
    /// 取得或设置流程业务数据ID。
    /// </summary>
    public string BizId { get; set; }

    /// <summary>
    /// 取得或设置业务单据状态。
    /// </summary>
    public string BizStatus { get; set; }

    /// <summary>
    /// 取得或设置提交给或指派给的用户账号。
    /// </summary>
    public string User { get; set; }

    /// <summary>
    /// 取得或设置提交给用户的角色查询条件。
    /// </summary>
    public string UserRole { get; set; }

    /// <summary>
    /// 取得或设置表单备注。
    /// </summary>
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置流程状态。
    /// </summary>
    public string FlowStatus { get; set; }

    /// <summary>
    /// 取得或设置流程表单对象。
    /// </summary>
    public object Model { get; set; }
}