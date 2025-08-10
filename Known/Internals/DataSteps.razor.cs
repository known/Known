using AntDesign;

namespace Known.Internals;

/// <summary>
/// 步骤组件类。
/// </summary>
public partial class DataSteps
{
    private StepsDirection Direction => Model.IsVertical ? StepsDirection.Vertical : StepsDirection.Horizontal;

    /// <summary>
    /// 取得或设置步骤配置模型。
    /// </summary>
    [Parameter] public StepModel Model { get; set; }
}