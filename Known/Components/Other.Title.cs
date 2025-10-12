namespace Known.Components;

/// <summary>
/// 标题组件类。
/// </summary>
public class KTitle : BaseComponent
{
    private string ClassName => CssBuilder.Default("kui-title").AddClass(Class).BuildClass();

    /// <summary>
    /// 取得或设置组件标题文本。
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// 取得或设置组件子标题文本。
    /// </summary>
    [Parameter] public string SubText { get; set; }

    /// <summary>
    /// 取得或设置标题组件子内容。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (!Visible)
            return;

        builder.Div().Class(ClassName).Style(Style).Child(() =>
        {
            if (ChildContent != null)
            {
                builder.Fragment(ChildContent);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(Text))
                    builder.Span(Language[Text]);
                if (!string.IsNullOrWhiteSpace(SubText))
                {
                    builder.Component<KLink>()
                           .Set(c => c.Class, "kui-sub-title")
                           .Set(c => c.Name, Language[SubText])
                           .Build();
                }
            }
        });
    }
}