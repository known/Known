namespace Known.Components;

/// <summary>
/// 标签组件类。
/// </summary>
public class KTag : BaseComponent
{
    /// <summary>
    /// 取得或设置标签文本。
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// 取得或设置标签颜色。
    /// </summary>
    [Parameter] public string Color { get; set; }

    /// <summary>
    /// 呈现标签组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        var text = Language?.GetCode(Text);
        if (string.IsNullOrWhiteSpace(text))
            text = Text;
        UI.BuildTag(builder, text, Color);
    }
}