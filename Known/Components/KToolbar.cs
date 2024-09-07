namespace Known.Components;

/// <summary>
/// 工具条组件类。
/// </summary>
public class KToolbar : BaseComponent
{
    /// <summary>
    /// 取得或设置工具条子内容。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// 呈现工具条组件内容。
    /// </summary>
    /// <param name="builder">呈现建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("kui-toolbar", () => ChildContent(builder));
    }
}