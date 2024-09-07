namespace Known.Components;

/// <summary>
/// 标题组件类。
/// </summary>
public class KTitle : BaseComponent
{
    /// <summary>
    /// 取得或设置组件标题文本。
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// 取得或设置标题组件子内容。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// 呈现标题组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (!string.IsNullOrWhiteSpace(Text))
            builder.Div("kui-title", Text);
        else if (ChildContent != null)
            builder.Div("kui-title", () => ChildContent(builder));
    }
}