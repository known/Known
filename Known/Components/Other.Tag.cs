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
    /// 取得或设置标签点击事件。
    /// </summary>
    [Parameter] public EventCallback OnClick { get; set; }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (!Visible)
            return;

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