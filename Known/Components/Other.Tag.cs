using AntDesign;

namespace Known.Components;

/// <summary>
/// 标签组件类。
/// </summary>
public class KTag : BaseComponent
{
    /// <summary>
    /// 取得或设置标签文本。
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// 取得或设置标签颜色。
    /// </summary>
    [Parameter] public string Color { get; set; }

    /// <summary>
    /// 取得或设置标签提示文本。
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// 取得或设置标签点击事件。
    /// </summary>
    [Parameter] public EventCallback OnClick { get; set; }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (!Visible)
            return;

        if (!string.IsNullOrWhiteSpace(Title))
            builder.Component<Tooltip>().Set(c => c.TitleTemplate, BuildTitle).Set(c => c.ChildContent, BuildTag).Build();
        else
            BuildTag(builder);
    }

    private void BuildTitle(RenderTreeBuilder builder)
    {
        builder.Markup(Language[Title]);
    }

    private void BuildTag(RenderTreeBuilder builder)
    {
        builder.Component<Tag>()
                       .Set(c => c.Color, GetColor(Text))
                       .Set(c => c.OnClick, OnClick)
                       .Set(c => c.ChildContent, b => b.Text(Language[Text]))
                       .Build();
    }

    private string GetColor(string text)
    {
        if (!string.IsNullOrWhiteSpace(Color))
            return Color;

        return UIService.GetTagColor(text);
    }
}