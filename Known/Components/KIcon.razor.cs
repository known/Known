using AntDesign;

namespace Known.Components;

/// <summary>
/// 图标组件类。
/// </summary>
public partial class KIcon
{
    /// <summary>
    /// 取得或设置图标。
    /// </summary>
    [Parameter] public string Icon { get; set; }

    /// <summary>
    /// 取得或设置图标名称。
    /// </summary>
    [Parameter] public string Name { get; set; }

    /// <summary>
    /// 取得或设置图标提示标题。
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// 取得或设置图标单击事件。
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs>? OnClick { get; set; }

    private RenderFragment TitleTemplate => b => b.Markup(Title);

    private RenderFragment RenderItem()
    {
        if (string.IsNullOrWhiteSpace(Name))
            return b => RenderIcon(b, OnClick);

        return b => b.Div().Class("kui-icon").OnClick(OnClick).Child(() =>
        {
            RenderIcon(b);
            b.Span("icon-name", Name);
        });
    }

    private void RenderIcon(RenderTreeBuilder builder, EventCallback<MouseEventArgs>? onClick = null)
    {
        if (Icon.StartsWith("fa"))
        {
            builder.Span().Class(Icon).OnClick(OnClick).Close();
            return;
        }

        if (onClick != null)
            builder.Component<Icon>().Set(c => c.Type, Icon).Set(c => c.OnClick, onClick.Value).Build();
        else
            builder.Component<Icon>().Set(c => c.Type, Icon).Build();
    }
}