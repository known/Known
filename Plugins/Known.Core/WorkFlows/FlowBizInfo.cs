namespace Known.WorkFlows;

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