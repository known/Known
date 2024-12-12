namespace Known.Extensions;

/// <summary>
/// 小碎片组件扩展类。
/// </summary>
public static class FragmentExtension
{
    /// <summary>
    /// 呈现一个提示框。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="text">提示文本。</param>
    /// <param name="type">提示框类型，默认Info。</param>
    public static void Alert(this RenderTreeBuilder builder, string text, StyleType type = StyleType.Info)
    {
        builder.Component<KAlert>()
               .Set(c => c.Text, text)
               .Set(c => c.Type, type)
               .Build();
    }

    /// <summary>
    /// 呈现一个标签组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="text">标签文本。</param>
    /// <param name="color">标签颜色。</param>
    public static void Tag(this RenderTreeBuilder builder, string text, string color = null)
    {
        builder.Component<KTag>()
               .Set(c => c.Text, text)
               .Set(c => c.Color, color)
               .Build();
    }

    /// <summary>
    /// 呈现一个图标组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="icon">图标。</param>
    /// <param name="onClick">图标单击事件。</param>
    public static void Icon(this RenderTreeBuilder builder, string icon, EventCallback<MouseEventArgs>? onClick = null)
    {
        if (string.IsNullOrWhiteSpace(icon))
            return;

        builder.Component<KIcon>()
               .Set(c => c.Icon, icon)
               .Set(c => c.OnClick, onClick)
               .Build();
    }

    /// <summary>
    /// 呈现一个图标和名称组件。
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="icon"></param>
    /// <param name="name"></param>
    public static void IconName(this RenderTreeBuilder builder, string icon, string name)
    {
        if (!string.IsNullOrWhiteSpace(icon))
        {
            builder.Component<KIcon>()
                   .Set(c => c.Icon, icon)
                   .Build();
        }
        builder.Span(name);
    }
}