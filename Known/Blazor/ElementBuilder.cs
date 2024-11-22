namespace Known.Blazor;

/// <summary>
/// HTML元素建造者类。
/// </summary>
public class ElementBuilder
{
    internal readonly RenderTreeBuilder Builder;

    internal ElementBuilder(RenderTreeBuilder builder, string elementName)
    {
        Builder = builder;
        builder.OpenElement(0, elementName);
    }

    /// <summary>
    /// 设置HTML元素的id属性。
    /// </summary>
    /// <param name="id">id属性值。</param>
    /// <returns>元素建造者。</returns>
    public ElementBuilder Id(string id) => Set("id", id);

    /// <summary>
    /// 设置HTML元素的class属性。
    /// </summary>
    /// <param name="className">class属性值。</param>
    /// <returns>元素建造者。</returns>
    public ElementBuilder Class(string className) => Set("class", className);

    /// <summary>
    /// 设置HTML元素的style属性。
    /// </summary>
    /// <param name="style">style属性值。</param>
    /// <returns>元素建造者。</returns>
    public ElementBuilder Style(string style) => Set("style", style);

    /// <summary>
    /// 设置HTML元素属性。
    /// </summary>
    /// <param name="name">属性名。</param>
    /// <param name="value">属性值。</param>
    /// <param name="checkNull">是否检查空值，空值不添加属性。</param>
    /// <returns>元素建造者。</returns>
    public ElementBuilder Set(string name, object value, bool checkNull = true)
    {
        if (checkNull && (value == null || string.IsNullOrWhiteSpace(value?.ToString())))
            return this;

        Builder.AddAttribute(1, name, value);
        return this;
    }

    /// <summary>
    /// 设置HTML字符串内容。
    /// </summary>
    /// <param name="markup">HTML字符串。</param>
    /// <returns>元素建造者。</returns>
    public ElementBuilder Markup(string markup)
    {
        Builder.Markup(markup);
        return this;
    }

    /// <summary>
    /// 设置HTML元素文本字符串。
    /// </summary>
    /// <param name="text">文本字符串。</param>
    /// <returns>元素建造者。</returns>
    public ElementBuilder Child(string text)
    {
        Builder.AddContent(1, text);
        return this;
    }

    /// <summary>
    /// 设置HTML元素子内容。
    /// </summary>
    /// <param name="child">子内容委托。</param>
    /// <returns>元素建造者。</returns>
    public ElementBuilder Child(Action child)
    {
        child.Invoke();
        return this;
    }

    /// <summary>
    /// 设置HTML元素带参数的子内容。
    /// </summary>
    /// <typeparam name="T">参数类型。</typeparam>
    /// <param name="child">子内容委托。</param>
    /// <param name="item">参数对象。</param>
    /// <returns>元素建造者。</returns>
    public ElementBuilder Child<T>(Action<T> child, T item)
    {
        child.Invoke(item);
        return this;
    }

    /// <summary>
    /// 关闭HTML元素标签。
    /// </summary>
    public void Close() => Builder.CloseElement();
}