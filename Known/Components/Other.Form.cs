namespace Known.Components;

/// <summary>
/// 表单组件类。
/// </summary>
public class KForm : BaseComponent
{
    private string ClassName => CssBuilder.Default("kui-form")
                                          .AddClass(Class)
                                          .AddClass("small-label", SmallLabel)
                                          .AddClass("small-row", SmallRow)
                                          .AddClass("readonly", ReadOnly)
                                          .BuildClass();

    /// <summary>
    /// 取得或设置是否是窄宽标题列。
    /// </summary>
    [Parameter] public bool SmallLabel { get; set; }

    /// <summary>
    /// 取得或设置是否是窄宽行高。
    /// </summary>
    [Parameter] public bool SmallRow { get; set; }

    /// <summary>
    /// 取得或设置子组件模板。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div(ClassName, () => builder.Fragment(ChildContent));
    }
}