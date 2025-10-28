namespace Known.WorkFlows;

/// <summary>
/// 工作流配置信息类。
/// </summary>
public class FlowInfo
{
    /// <summary>
    /// 取得或设置工作流ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置工作流名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置工作流描述。
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 取得或设置工作流表单对话框宽度。
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// 取得或设置工作流表单对话框高度。
    /// </summary>
    public int? Height { get; set; }

    /// <summary>
    /// 取得或设置工作流当前步骤。
    /// </summary>
    public int Current { get; set; }

    /// <summary>
    /// 取得或设置工作流步骤列表。
    /// </summary>
    public List<FlowStepInfo> Steps { get; set; } = [];
}