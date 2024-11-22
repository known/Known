namespace Known.Extensions;

/// <summary>
/// HTML元素事件扩展类。
/// </summary>
public static class HtmlEventExtension
{
    /// <summary>
    /// 给HTML元素添加可拖拽属性。
    /// </summary>
    /// <param name="builder">元素建造者。</param>
    /// <returns>元素建造者。</returns>
    public static ElementBuilder Draggable(this ElementBuilder builder)
    {
        return builder.Set("draggable", "true").Set("ondragover", "event.preventDefault()");
    }

    /// <summary>
    /// 给HTML元素添加ondrop属性。
    /// </summary>
    /// <param name="builder">元素建造者。</param>
    /// <param name="onDrop">ondrop属性值。</param>
    /// <returns>元素建造者。</returns>
    public static ElementBuilder OnDrop(this ElementBuilder builder, EventCallback<DragEventArgs> onDrop) => builder.Set("ondrop", onDrop);

    /// <summary>
    /// 给HTML元素添加ondragstart属性。
    /// </summary>
    /// <param name="builder">元素建造者。</param>
    /// <param name="onDragStart">ondragstart属性值。</param>
    /// <returns>元素建造者。</returns>
    public static ElementBuilder OnDragStart(this ElementBuilder builder, EventCallback<DragEventArgs> onDragStart) => builder.Set("ondragstart", onDragStart);

    /// <summary>
    /// 给HTML元素添加onclick属性。
    /// </summary>
    /// <param name="builder">元素建造者。</param>
    /// <param name="onClick">onclick属性值。</param>
    /// <returns>元素建造者。</returns>
    public static ElementBuilder OnClick(this ElementBuilder builder, object onClick) => builder.Set("onclick", onClick);

    /// <summary>
    /// 阻止HTML元素的onclick属性的默认事件。
    /// </summary>
    /// <param name="builder">元素建造者。</param>
    /// <returns>元素建造者。</returns>
    public static ElementBuilder PreventDefault(this ElementBuilder builder)
    {
        builder.Builder.AddEventPreventDefaultAttribute(1, "onclick", value: true);
        return builder;
    }
}