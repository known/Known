namespace Known.Extensions;

/// <summary>
/// HTML元素呈现扩展类。
/// </summary>
public static class ElementExtension
{
    /// <summary>
    /// 呈现一个HTML元素的开头标签。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="elementName">HTML元素名。</param>
    /// <returns>呈现树建造者。</returns>
    public static RenderTreeBuilder OpenElement(this RenderTreeBuilder builder, string elementName)
    {
        builder.OpenElement(0, elementName);
        return builder;
    }

    /// <summary>
    /// 呈现一个div元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <returns>呈现树建造者。</returns>
    public static RenderTreeBuilder Div(this RenderTreeBuilder builder) => builder.OpenElement("div");

    /// <summary>
    /// 呈现一个label元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <returns>呈现树建造者。</returns>
    public static RenderTreeBuilder Label(this RenderTreeBuilder builder) => builder.OpenElement("label");

    /// <summary>
    /// 呈现一个span元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <returns>呈现树建造者。</returns>
    public static RenderTreeBuilder Span(this RenderTreeBuilder builder) => builder.OpenElement("span");

    /// <summary>
    /// 呈现一个ul元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <returns>呈现树建造者。</returns>
    public static RenderTreeBuilder Ul(this RenderTreeBuilder builder) => builder.OpenElement("ul");

    /// <summary>
    /// 呈现一个li元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <returns>呈现树建造者。</returns>
    public static RenderTreeBuilder Li(this RenderTreeBuilder builder) => builder.OpenElement("li");

    /// <summary>
    /// 呈现一个pre元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <returns>呈现树建造者。</returns>
    public static RenderTreeBuilder Pre(this RenderTreeBuilder builder) => builder.OpenElement("pre");

    /// <summary>
    /// 呈现一个a元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <returns>呈现树建造者。</returns>
    public static RenderTreeBuilder Link(this RenderTreeBuilder builder) => builder.OpenElement("a");

    /// <summary>
    /// 呈现一个image元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <returns>呈现树建造者。</returns>
    public static RenderTreeBuilder Image(this RenderTreeBuilder builder) => builder.OpenElement("img");

    /// <summary>
    /// 呈现一个canvas元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <returns>呈现树建造者。</returns>
    public static RenderTreeBuilder Canvas(this RenderTreeBuilder builder) => builder.OpenElement("canvas");

    /// <summary>
    /// 呈现一个iframe元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <returns>呈现树建造者。</returns>
    public static RenderTreeBuilder IFrame(this RenderTreeBuilder builder) => builder.OpenElement("iframe");

    /// <summary>
    /// 呈现HTML元素的属性。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="name">属性名。</param>
    /// <param name="value">属性值。</param>
    /// <param name="checkNull">检查属性值是否为空，空则不呈现，默认检查。</param>
    /// <returns>呈现树建造者。</returns>
    public static RenderTreeBuilder Attribute(this RenderTreeBuilder builder, string name, object value, bool checkNull = true)
    {
        if (checkNull && (value == null || string.IsNullOrWhiteSpace(value?.ToString())))
            return builder;

        builder.AddAttribute(1, name, value);
        return builder;
    }

    /// <summary>
    /// 呈现一个HTML元素的id属性。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="id">id属性值。</param>
    /// <returns>呈现树建造者。</returns>
    public static RenderTreeBuilder Id(this RenderTreeBuilder builder, string id) => builder.Attribute("id", id);

    /// <summary>
    /// 呈现一个HTML元素的class属性。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="className">class属性值。</param>
    /// <returns>呈现树建造者。</returns>
    public static RenderTreeBuilder Class(this RenderTreeBuilder builder, string className) => builder.Attribute("class", className);

    /// <summary>
    /// 呈现一个HTML元素的title属性。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="title">title属性值。</param>
    /// <returns>呈现树建造者。</returns>
    public static RenderTreeBuilder Title(this RenderTreeBuilder builder, string title) => builder.Attribute("title", title);

    /// <summary>
    /// 呈现一个HTML元素的style属性。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="style">style属性值。</param>
    /// <returns>呈现树建造者。</returns>
    public static RenderTreeBuilder Style(this RenderTreeBuilder builder, string style) => builder.Attribute("style", style);

    /// <summary>
    /// 呈现一个HTML元素的href属性。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="href">href属性值。</param>
    /// <returns>呈现树建造者。</returns>
    public static RenderTreeBuilder Href(this RenderTreeBuilder builder, string href) => builder.Attribute("href", href);

    /// <summary>
    /// 呈现一个HTML元素的src属性。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="src">src属性值。</param>
    /// <returns>呈现树建造者。</returns>
    public static RenderTreeBuilder Src(this RenderTreeBuilder builder, string src) => builder.Attribute("src", src);

    /// <summary>
    /// 呈现一个HTML元素的role属性。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="role">role属性值。</param>
    /// <returns>呈现树建造者。</returns>
    public static RenderTreeBuilder Role(this RenderTreeBuilder builder, string role) => builder.Attribute("role", role);

    /// <summary>
    /// 给HTML元素添加可拖拽属性。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <returns>呈现树建造者。</returns>
    public static RenderTreeBuilder Draggable(this RenderTreeBuilder builder)
    {
        return builder.Attribute("draggable", "true").Attribute("ondragover", "event.preventDefault()");
    }

    /// <summary>
    /// 给HTML元素添加ondrop属性。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="onDrop">ondrop属性值。</param>
    /// <returns>呈现树建造者。</returns>
    public static RenderTreeBuilder OnDrop(this RenderTreeBuilder builder, EventCallback<DragEventArgs> onDrop) => builder.Attribute("ondrop", onDrop);

    /// <summary>
    /// 给HTML元素添加ondragstart属性。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="onDragStart">ondragstart属性值。</param>
    /// <returns>呈现树建造者。</returns>
    public static RenderTreeBuilder OnDragStart(this RenderTreeBuilder builder, EventCallback<DragEventArgs> onDragStart) => builder.Attribute("ondragstart", onDragStart);

    /// <summary>
    /// 给HTML元素添加onclick属性。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="onClick">onclick属性值。</param>
    /// <returns>呈现树建造者。</returns>
    public static RenderTreeBuilder OnClick(this RenderTreeBuilder builder, object onClick) => builder.Attribute("onclick", onClick);

    /// <summary>
    /// 阻止HTML元素的onclick属性的默认事件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <returns>呈现树建造者。</returns>
    public static RenderTreeBuilder PreventDefault(this RenderTreeBuilder builder)
    {
        builder.AddEventPreventDefaultAttribute(1, "onclick", value: true);
        return builder;
    }

    /// <summary>
    /// 呈现一个HTML元素的子元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="child">子元素呈现委托。</param>
    /// <returns>呈现树建造者。</returns>
    public static RenderTreeBuilder Children(this RenderTreeBuilder builder, Action child)
    {
        child.Invoke();
        return builder;
    }

    /// <summary>
    /// 呈现一个HTML元素的关闭标签。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    public static void Close(this RenderTreeBuilder builder) => builder.CloseElement();
}