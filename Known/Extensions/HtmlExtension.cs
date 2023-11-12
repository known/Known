using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Extensions;

public static class HtmlExtension
{
    public static void Div(this RenderTreeBuilder builder, Action child) => builder.Div("", child);
    public static void Div(this RenderTreeBuilder builder, string className, string text) => builder.Div(className, () => builder.AddContent(2, text));
    public static void Div(this RenderTreeBuilder builder, string className, Action child)
    {
        builder.OpenElement(0, "div");
        if (!string.IsNullOrEmpty(className))
            builder.AddAttribute(1, "class", className);
        child?.Invoke();
        builder.CloseElement();
    }

    public static void Span(this RenderTreeBuilder builder, string text) => builder.Span("", text);
    public static void Span(this RenderTreeBuilder builder, string className, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        builder.OpenElement(0, "span");
        if (!string.IsNullOrWhiteSpace(className))
            builder.AddAttribute(1, "class", className);
        builder.AddContent(1, text);
        builder.CloseElement();
    }

    public static void Link(this RenderTreeBuilder builder, string text, EventCallback onClick)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        builder.OpenElement(0, "span");
        builder.AddAttribute(1, "class", "link");
        builder.AddAttribute(2, "onclick", onClick);
        builder.AddContent(3, text);
        builder.CloseElement();
    }
}