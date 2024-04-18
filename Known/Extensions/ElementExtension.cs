namespace Known.Extensions;

public static class ElementExtension
{
    public static RenderTreeBuilder OpenElement(this RenderTreeBuilder builder, string elementName)
    {
        builder.OpenElement(0, elementName);
        return builder;
    }

    public static RenderTreeBuilder Div(this RenderTreeBuilder builder) => builder.OpenElement("div");
    public static RenderTreeBuilder Label(this RenderTreeBuilder builder) => builder.OpenElement("label");
    public static RenderTreeBuilder Span(this RenderTreeBuilder builder) => builder.OpenElement("span");
    public static RenderTreeBuilder Ul(this RenderTreeBuilder builder) => builder.OpenElement("ul");
    public static RenderTreeBuilder Li(this RenderTreeBuilder builder) => builder.OpenElement("li");
    public static RenderTreeBuilder Image(this RenderTreeBuilder builder) => builder.OpenElement("img");
    public static RenderTreeBuilder Canvas(this RenderTreeBuilder builder) => builder.OpenElement("canvas");
    public static RenderTreeBuilder IFrame(this RenderTreeBuilder builder) => builder.OpenElement("iframe");

    public static RenderTreeBuilder Attribute(this RenderTreeBuilder builder, string name, object value, bool checkNull = false)
    {
        if (checkNull && value == null)
            return builder;

        builder.AddAttribute(1, name, value);
        return builder;
    }

    public static RenderTreeBuilder Id(this RenderTreeBuilder builder, string id) => builder.Attribute("id", id);
    public static RenderTreeBuilder Class(this RenderTreeBuilder builder, string className) => builder.Attribute("class", className);
    public static RenderTreeBuilder Title(this RenderTreeBuilder builder, string title) => builder.Attribute("title", title);
    public static RenderTreeBuilder Style(this RenderTreeBuilder builder, string style) => builder.Attribute("style", style);
    public static RenderTreeBuilder Src(this RenderTreeBuilder builder, string src) => builder.Attribute("src", src);
    public static RenderTreeBuilder Role(this RenderTreeBuilder builder, string role) => builder.Attribute("role", role);
    public static RenderTreeBuilder OnClick(this RenderTreeBuilder builder, object onclick) => builder.Attribute("onclick", onclick, true);

    public static RenderTreeBuilder PreventDefault(this RenderTreeBuilder builder)
    {
        builder.AddEventPreventDefaultAttribute(1, "onclick", value: true);
        return builder;
    }

    public static RenderTreeBuilder Children(this RenderTreeBuilder builder, Action child)
    {
        child.Invoke();
        return builder;
    }

    public static void Close(this RenderTreeBuilder builder) => builder.CloseElement();
}