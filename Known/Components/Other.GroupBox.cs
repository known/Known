namespace Known.Components;

/// <summary>
/// 分组框组件类。
/// </summary>
public class KGroupBox : BaseComponent
{
    /// <summary>
    /// 取得或设置分组框标题。
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// 取得或设置分组框子内容。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (!Visible)
            return;

        builder.Div("kui-group-box", () =>
        {
            builder.Label("legend", Language[Title]);
            builder.Div("body", () => builder.Fragment(ChildContent));
        });
    }
}