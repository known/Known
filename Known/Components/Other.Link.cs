namespace Known.Components;

/// <summary>
/// 连接组件类。
/// </summary>
public class KLink : BaseComponent
{
    /// <summary>
    /// 取得或设置连接点击事件委托。
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <summary>
    /// 取得或设置连接子内容模板。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        var className = CssBuilder.Default("ant-btn-link").AddClass(Class).BuildClass();
        builder.Span().Class(className).Style(Style).OnClick(OnClick).Child(() =>
        {
            if (ChildContent != null)
                builder.Fragment(ChildContent);
            else
                builder.Text(Name);
        });
    }
}