namespace Known.WorkFlows;

/// <summary>
/// 工作流步骤配置信息类。
/// </summary>
public class FlowStepInfo
{
    /// <summary>
    /// 取得或设置步骤ID。
    /// </summary>
    [DisplayName("Id")]
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置步骤名称。
    /// </summary>
    [DisplayName("Name")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置步骤操作用户。
    /// </summary>
    [DisplayName("User")]
    public string User { get; set; }

    /// <summary>
    /// 取得或设置步骤操作角色。
    /// </summary>
    [DisplayName("Role")]
    public string Role { get; set; }

    /// <summary>
    /// 取得或设置步骤审核通过的流程状态。
    /// </summary>
    [DisplayName("Pass")]
    public string Pass { get; set; }

    /// <summary>
    /// 取得或设置步骤审核退回的流程状态。
    /// </summary>
    [DisplayName("Fail")]
    public string Fail { get; set; }
}