namespace Known.Components;

/// <summary>
/// 行组件类，默认平分行内子元素。
/// </summary>
public class KRow : BaseComponent
{
    /// <summary>
    /// 取得或设置行内两列左右百分比尺寸(19,28,37,46,55,64,73,82,91)。
    /// </summary>
    [Parameter] public string Size { get; set; }

    /// <summary>
    /// 取得或设置子组件模板。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        var css = "kui-row";
        if (!string.IsNullOrWhiteSpace(Size))
            css += $"-{Size}";
        var className = CssBuilder.Default(css).AddClass(Class).BuildClass();
        builder.Div(className, () => builder.Fragment(ChildContent));
    }
}