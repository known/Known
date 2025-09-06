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
    /// 取得或设置图标提示标题。
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// 取得或设置图标颜色是否跟随主题色。
    /// </summary>
    [Parameter] public bool IsTheme { get; set; }

    /// <summary>
    /// 取得或设置图标单击事件。
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs>? OnClick { get; set; }

    private RenderFragment RenderItem()
    {
        if (string.IsNullOrWhiteSpace(Name))
            return b => BuildIcon(b, OnClick);

        return b => b.Div().Class("kui-icon").OnClick(OnClick).Child(() =>
        {
            BuildIcon(b);
            b.Span("icon-name", Language[Name]);
        });
    }

    private void BuildIcon(RenderTreeBuilder builder, EventCallback<MouseEventArgs>? onClick = null)
    {
        if (string.IsNullOrWhiteSpace(Icon))
            return;

        if (Utils.CheckImage(Icon))
        {
            builder.Image().Src(Icon).Close();
            return;
        }

        var className = CssBuilder.Default(Icon.StartsWith("fa") ? Icon : "")
                                  .AddClass("ant-btn-link", IsTheme)
                                  .BuildClass();
        if (Icon.StartsWith("fa"))
        {
            builder.Span().Class(className).OnClick(OnClick).Close();
            return;
        }

        BuildIcon(builder, className, onClick);
    }

    private void BuildIcon(RenderTreeBuilder builder, string className, EventCallback<MouseEventArgs>? onClick)
    {
        var icons = Icon.Split(',');
        var theme = IconThemeType.Outline;
        var icon = icons[0];
        if (icons.Length > 1)
        {
            theme = Utils.ConvertTo<IconThemeType>(icons[0]);
            icon = icons[1];
        }
        if (onClick != null)
            builder.Component<Icon>()
                   .Set(c => c.Class, className)
                   .Set(c => c.Type, icon)
                   .Set(c => c.Theme, theme)
                   .Set(c => c.OnClick, onClick.Value)
                   .Build();
        else
            builder.Component<Icon>()
                   .Set(c => c.Class, className)
                   .Set(c => c.Type, icon)
                   .Set(c => c.Theme, theme)
                   .Build();
    }
}