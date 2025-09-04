using AntDesign;

namespace Known.Extensions;

/// <summary>
/// 组件建造者扩展类。
/// </summary>
public static class BuilderExtension
{
    /// <summary>
    /// 创建HTML元素建造者。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="elementName">HTML元素名称。</param>
    /// <returns>HTML元素建造者。</returns>
    public static ElementBuilder Element(this RenderTreeBuilder builder, string elementName)
    {
        return new ElementBuilder(builder, elementName);
    }

    /// <summary>
    /// 创建组件建造者。
    /// </summary>
    /// <typeparam name="T">组件类型。</typeparam>
    /// <param name="builder">呈现树建造者。</param>
    /// <returns>组件建造者。</returns>
    public static ComponentBuilder<T> Component<T>(this RenderTreeBuilder builder) where T : notnull, Microsoft.AspNetCore.Components.IComponent
    {
        return new ComponentBuilder<T>(builder);
    }

    /// <summary>
    /// 创建水印组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="user">当前用户。</param>
    /// <param name="content">组件内容。</param>
    public static void Watermark(this RenderTreeBuilder builder, UserInfo user, RenderFragment content)
    {
        var text = user?.Watermark ?? $"{user?.Name}({user?.UserName})";
        builder.Watermark(text, content);
    }

    /// <summary>
    /// 创建水印组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="text">水印文字。</param>
    /// <param name="content">组件内容。</param>
    public static void Watermark(this RenderTreeBuilder builder, string text, RenderFragment content)
    {
        builder.Component<Watermark>()
               .Set(c => c.Content, text)
               .Set(c => c.ChildContent, content)
               .Build();
    }

    internal static void BuildBody(this RenderTreeBuilder builder, Context context, RenderFragment body)
    {
        if (Config.System?.IsWatermark == true && context != null)
            builder.Watermark(context.CurrentUser, body);
        else
            builder.Fragment(body);
    }
}