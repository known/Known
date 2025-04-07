namespace Known.Components;

/// <summary>
/// 胶囊组件类。
/// </summary>
public class KCapsule : BaseComponent
{
    /// <summary>
    /// 取得或设置胶囊组件的值。
    /// </summary>
    [Parameter] public string Value { get; set; }

    /// <summary>
    /// 取得或设置胶囊组件值的背景颜色。
    /// </summary>
    [Parameter] public string Color { get; set; }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("kui-capsule", () =>
        {
            builder.Span(Name);
            builder.Span().Class("kui-capsule-value").Style($"background-color:{Color};").Child(Value);
        });
    }
}