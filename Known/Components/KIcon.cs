namespace Known.Components;

/// <summary>
/// 图标组件类。
/// </summary>
public class KIcon : BaseComponent
{
    /// <summary>
    /// 取得或设置图标。
    /// </summary>
    [Parameter] public string Icon { get; set; }

    /// <summary>
    /// 取得或设置图标单击事件。
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs>? OnClick { get; set; }

    /// <summary>
    /// 呈现图标组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (Icon.StartsWith("fa"))
        {
            builder.Span(Icon, "", OnClick);
            return;
        }

        UI.BuildIcon(builder, Icon, OnClick);
    }
}