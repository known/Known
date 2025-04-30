namespace Known.Components;

/// <summary>
/// 标签组件类。
/// </summary>
public partial class KTag
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

    private string GetText()
    {
        var text = Language.GetCode(Text);
        if (string.IsNullOrWhiteSpace(text))
            text = Language[Text];
        return text;
    }

    private string GetColor(string text)
    {
        if (!string.IsNullOrWhiteSpace(Color))
            return Color;

        return UIService.GetTagColor(text);
    }
}