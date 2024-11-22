namespace Known.Extensions;

/// <summary>
/// HTML元素属性扩展类。
/// </summary>
public static class HtmlAttributeExtension
{
    /// <summary>
    /// 呈现一个HTML元素的title属性。
    /// </summary>
    /// <param name="builder">元素建造者。</param>
    /// <param name="title">title属性值。</param>
    /// <returns>元素建造者。</returns>
    public static ElementBuilder Title(this ElementBuilder builder, string title) => builder.Set("title", title);

    /// <summary>
    /// 呈现一个HTML元素的href属性。
    /// </summary>
    /// <param name="builder">元素建造者。</param>
    /// <param name="href">href属性值。</param>
    /// <returns>元素建造者。</returns>
    public static ElementBuilder Href(this ElementBuilder builder, string href) => builder.Set("href", href);

    /// <summary>
    /// 呈现一个HTML元素的src属性。
    /// </summary>
    /// <param name="builder">元素建造者。</param>
    /// <param name="src">src属性值。</param>
    /// <returns>元素建造者。</returns>
    public static ElementBuilder Src(this ElementBuilder builder, string src) => builder.Set("src", src);

    /// <summary>
    /// 呈现一个HTML元素的role属性。
    /// </summary>
    /// <param name="builder">元素建造者。</param>
    /// <param name="role">role属性值。</param>
    /// <returns>元素建造者。</returns>
    public static ElementBuilder Role(this ElementBuilder builder, string role) => builder.Set("role", role);
}