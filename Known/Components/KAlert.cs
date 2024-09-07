namespace Known.Components;

/// <summary>
/// 提示框组件类。
/// </summary>
public class KAlert : BaseComponent
{
    /// <summary>
    /// 取得或设置提示框类型。
    /// </summary>
    [Parameter] public StyleType Type { get; set; }

    /// <summary>
    /// 取得或设置提示框文本。
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// 呈现提示框组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        UI.BuildAlert(builder, Text, Type);
    }
}