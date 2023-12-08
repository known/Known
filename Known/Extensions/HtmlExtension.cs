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

    public static void Ul(this RenderTreeBuilder builder, Action child) => builder.Ul("", child);
    public static void Ul(this RenderTreeBuilder builder, string className, Action child)
    {
        builder.OpenElement(0, "ul");
        if (!string.IsNullOrEmpty(className))
            builder.AddAttribute(1, "class", className);
        child?.Invoke();
        builder.CloseElement();
    }

    public static void Li(this RenderTreeBuilder builder, Action child) => builder.Li("", child);
    public static void Li(this RenderTreeBuilder builder, string className, string text) => builder.Li(className, () => builder.AddContent(2, text));
    public static void Li(this RenderTreeBuilder builder, string className, Action child)
    {
        builder.OpenElement(0, "li");
        if (!string.IsNullOrEmpty(className))
            builder.AddAttribute(1, "class", className);
        child?.Invoke();
        builder.CloseElement();
    }

    public static void Span(this RenderTreeBuilder builder, string text, EventCallback? onClick = null) => builder.Span("", text, onClick);
    public static void Span(this RenderTreeBuilder builder, string className, string text, EventCallback? onClick = null)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        builder.OpenElement(0, "span");
        if (!string.IsNullOrWhiteSpace(className))
            builder.AddAttribute(1, "class", className);
        if (onClick != null)
            builder.AddAttribute(1, "onclick", onClick);
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

    public static void DownloadLink(this RenderTreeBuilder builder, string text, FileUrlInfo url)
    {
        builder.OpenElement(0, "a");
        builder.AddAttribute(1, "href", url.OriginalUrl);
        builder.AddAttribute(2, "target", "_blank");
        builder.AddAttribute(3, "download", url.FileName);
        builder.AddContent(4, text);
        builder.CloseElement();
    }
}