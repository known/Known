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
}