namespace Known.Components;

/// <summary>
/// 垂直弹性布局。
/// </summary>
public class KFlexRow : BaseComponent
{
    /// <summary>
    /// 取得或设置子组件模板。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        var className = CssBuilder.Default("kui-flex-row").AddClass(Class).BuildClass();
        builder.Div().Class(className).Style(Style).Child(() => builder.Fragment(ChildContent));
    }
}

/// <summary>
/// 水平弹性布局。
/// </summary>
public class KFlexColumn : BaseComponent
{
    /// <summary>
    /// 取得或设置子组件模板。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        var className = CssBuilder.Default("kui-flex-column").AddClass(Class).BuildClass();
        builder.Div().Class(className).Style(Style).Child(() => builder.Fragment(ChildContent));
    }
}

/// <summary>
/// 两端弹性布局。
/// </summary>
public class KFlexSpace : BaseComponent
{
    /// <summary>
    /// 取得或设置子组件模板。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        var className = CssBuilder.Default("kui-flex-space").AddClass(Class).BuildClass();
        builder.Div().Class(className).Style(Style).Child(() => builder.Fragment(ChildContent));
    }
}