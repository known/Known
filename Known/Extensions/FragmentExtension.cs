using AntDesign;

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
        builder.Component<KAlert>().Set(c => c.Text, text).Set(c => c.Type, type).Build();
    }

    /// <summary>
    /// 呈现下拉弹出层组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="action">组件内容。</param>
    public static void Overlay(this RenderTreeBuilder builder, Action action)
    {
        builder.Div("kui-card overlay", action);
    }

    /// <summary>
    /// 呈现下拉弹出层组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="className">组件样式。</param>
    /// <param name="action">组件内容。</param>
    public static void Overlay(this RenderTreeBuilder builder, string className, Action action)
    {
        builder.Div($"kui-card overlay {className}", action);
    }

    /// <summary>
    /// 呈现工具条组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="action">组件内容。</param>
    public static void Toolbar(this RenderTreeBuilder builder, Action action)
    {
        builder.Div("kui-toolbar", action);
    }

    /// <summary>
    /// 呈现工具条组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">组件模型对象。</param>
    public static void Toolbar(this RenderTreeBuilder builder, ToolbarModel model)
    {
        builder.Component<KToolbar>().Set(c => c.Model, model).Build();
    }

    /// <summary>
    /// 呈现一个文字提示。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="title">提示文字。</param>
    /// <param name="childContent">子组件。</param>
    public static void Tooltip(this RenderTreeBuilder builder, string title, RenderFragment childContent)
    {
        builder.Component<Tooltip>().Set(c => c.Title, title).Set(c => c.ChildContent, childContent).Build();
    }

    /// <summary>
    /// 呈现一个标签组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="text">标签文本。</param>
    /// <param name="color">标签颜色。</param>
    public static void Tag(this RenderTreeBuilder builder, string text, string color = null)
    {
        builder.Component<KTag>().Set(c => c.Text, text).Set(c => c.Color, color).Build();
    }

    /// <summary>
    /// 呈现一个标签组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="text">标签文本。</param>
    /// <param name="title">标签提示文本。</param>
    /// <param name="color">标签颜色。</param>
    public static void Tag(this RenderTreeBuilder builder, string text, string title, string color = null)
    {
        builder.Component<KTag>().Set(c => c.Text, text).Set(c => c.Title, title).Set(c => c.Color, color).Build();
    }

    /// <summary>
    /// 呈现一个标签组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="text">标签文本。</param>
    /// <param name="onClick">标签点击事件。</param>
    /// <param name="color">标签颜色。</param>
    public static void Tag(this RenderTreeBuilder builder, string text, EventCallback onClick, string color = null)
    {
        builder.Component<KTag>()
               .Set(c => c.Text, text)
               .Set(c => c.Color, color)
               .Set(c => c.OnClick, onClick)
               .Build();
    }

    /// <summary>
    /// 呈现一个标签组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="texts">标签文本集合。</param>
    public static void Tags(this RenderTreeBuilder builder, params string[] texts)
    {
        foreach (var text in texts)
        {
            builder.Component<KTag>().Set(c => c.Text, text).Build();
        }
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

        builder.Component<KIcon>().Set(c => c.Icon, icon).Set(c => c.OnClick, onClick).Build();
    }

    /// <summary>
    /// 呈现一个图标组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="icon">图标。</param>
    /// <param name="title">提示标题。</param>
    /// <param name="onClick">图标单击事件。</param>
    public static void Icon(this RenderTreeBuilder builder, string icon, string title, EventCallback<MouseEventArgs>? onClick = null)
    {
        if (string.IsNullOrWhiteSpace(icon))
            return;

        builder.Component<KIcon>().Set(c => c.Icon, icon).Set(c => c.Title, title).Set(c => c.OnClick, onClick).Build();
    }

    /// <summary>
    /// 呈现一个图标和名称组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="icon">图标。</param>
    /// <param name="name">名称。</param>
    /// <param name="onClick">图标单击事件。</param>
    public static void IconName(this RenderTreeBuilder builder, string icon, string name, EventCallback<MouseEventArgs>? onClick = null)
    {
        builder.Component<KIcon>().Set(c => c.Icon, icon).Set(c => c.Name, name).Set(c => c.OnClick, onClick).Build();
    }
}