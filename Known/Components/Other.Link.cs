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
        builder.Span().Class("ant-btn-link").OnClick(OnClick).Child(() =>
        {
            if (ChildContent != null)
                builder.AddContent(0, ChildContent);
            else
                builder.AddContent(0, Name);
        });
    }
}