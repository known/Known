namespace Known.WorkFlows;

/// <summary>
/// 工作流步骤配置信息类。
/// </summary>
public class FlowStepInfo
{
    /// <summary>
    /// 取得或设置步骤ID。
    /// </summary>
    [DisplayName("步骤ID")]
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置步骤名称。
    /// </summary>
    [DisplayName("步骤名")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置步骤操作用户。
    /// </summary>
    [DisplayName("操作用户")]
    public string User { get; set; }

    /// <summary>
    /// 取得或设置步骤操作角色。
    /// </summary>
    [DisplayName("操作角色")]
    public string Role { get; set; }

    /// <summary>
    /// 取得或设置步骤审核通过的流程状态。
    /// </summary>
    [DisplayName("审核通过")]
    public string Pass { get; set; }

    /// <summary>
    /// 取得或设置步骤审核退回的流程状态。
    /// </summary>
    [DisplayName("审核退回")]
    public string Fail { get; set; }
}