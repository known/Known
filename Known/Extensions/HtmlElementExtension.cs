namespace Known.Extensions;

/// <summary>
/// HTML元素呈现扩展类。
/// </summary>
public static class HtmlElementExtension
{
    /// <summary>
    /// 呈现一个div元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <returns>元素建造者。</returns>
    public static ElementBuilder Div(this RenderTreeBuilder builder) => builder.Element("div");

    /// <summary>
    /// 呈现一个label元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <returns>元素建造者。</returns>
    public static ElementBuilder Label(this RenderTreeBuilder builder) => builder.Element("label");

    /// <summary>
    /// 呈现一个span元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <returns>元素建造者。</returns>
    public static ElementBuilder Span(this RenderTreeBuilder builder) => builder.Element("span");

    /// <summary>
    /// 呈现一个ul元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <returns>元素建造者。</returns>
    public static ElementBuilder Ul(this RenderTreeBuilder builder) => builder.Element("ul");

    /// <summary>
    /// 呈现一个li元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <returns>元素建造者。</returns>
    public static ElementBuilder Li(this RenderTreeBuilder builder) => builder.Element("li");

    /// <summary>
    /// 呈现一个pre元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <returns>元素建造者。</returns>
    public static ElementBuilder Pre(this RenderTreeBuilder builder) => builder.Element("pre");

    /// <summary>
    /// 呈现一个a元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <returns>元素建造者。</returns>
    public static ElementBuilder Link(this RenderTreeBuilder builder) => builder.Element("a");

    /// <summary>
    /// 呈现一个image元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <returns>元素建造者。</returns>
    public static ElementBuilder Image(this RenderTreeBuilder builder) => builder.Element("img");

    /// <summary>
    /// 呈现一个canvas元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <returns>元素建造者。</returns>
    public static ElementBuilder Canvas(this RenderTreeBuilder builder) => builder.Element("canvas");

    /// <summary>
    /// 呈现一个iframe元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <returns>元素建造者。</returns>
    public static ElementBuilder IFrame(this RenderTreeBuilder builder) => builder.Element("iframe");
}