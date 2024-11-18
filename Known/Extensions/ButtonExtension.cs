namespace Known.Extensions;

/// <summary>
/// 按钮组件扩展类。
/// </summary>
public static class ButtonExtension
{
    /// <summary>
    /// 构建按钮组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="info">操作按钮信息对象。</param>
    public static void Button(this RenderTreeBuilder builder, ActionInfo info)
    {
        builder.Component<KButton>()
               .Set(c => c.Enabled, info.Enabled)
               .Set(c => c.Icon, info.Icon)
               .Set(c => c.Name, info.Name)
               .Set(c => c.Type, info.Style)
               .Set(c => c.OnClick, info.OnClick)
               .Build();
    }

    /// <summary>
    /// 呈现一个按钮。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="action">操作信息。</param>
    /// <param name="onClick">按钮单击事件。</param>
    public static void Button(this RenderTreeBuilder builder, ActionInfo action, EventCallback<MouseEventArgs> onClick)
    {
        builder.Component<KButton>()
               .Set(c => c.Id, action.Id)
               .Set(c => c.Name, action.Name)
               .Set(c => c.Icon, action.Icon)
               .Set(c => c.Type, action.Style)
               .Set(c => c.Enabled, action.Enabled)
               .Set(c => c.Visible, action.Visible)
               .Set(c => c.OnClick, onClick)
               .Build();
    }

    /// <summary>
    /// 呈现一个按钮。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="name">按钮名称。</param>
    /// <param name="onClick">按钮单击事件。</param>
    /// <param name="type">按钮样式，默认primary。</param>
    public static void Button(this RenderTreeBuilder builder, string name, EventCallback<MouseEventArgs> onClick, string type = "primary")
    {
        builder.Component<KButton>()
               .Set(c => c.Name, name)
               .Set(c => c.Type, type)
               .Set(c => c.OnClick, onClick)
               .Build();
    }
}