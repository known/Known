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

/// <summary>
/// 工作流业务信息类。
/// </summary>
public class FlowBizInfo
{
    /// <summary>
    /// 取得或设置工作流代码。
    /// </summary>
    public string FlowCode { get; set; }

    /// <summary>
    /// 取得或设置工作流名称。
    /// </summary>
    public string FlowName { get; set; }

    /// <summary>
    /// 取得或设置工作流业务数据ID。
    /// </summary>
    public string BizId { get; set; }

    /// <summary>
    /// 取得或设置工作流业务名称。
    /// </summary>
    public string BizName { get; set; }

    /// <summary>
    /// 取得或设置工作流业务跳转URL。
    /// </summary>
    public string BizUrl { get; set; }

    /// <summary>
    /// 取得或设置工作流业务状态。
    /// </summary>
    public string BizStatus { get; set; }

    /// <summary>
    /// 取得或设置工作流当前操作用户。
    /// </summary>
    public UserInfo CurrUser { get; set; }
}