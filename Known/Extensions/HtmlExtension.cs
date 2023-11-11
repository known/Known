using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Extensions;

public static class HtmlExtension
{
    public static void Span(this RenderTreeBuilder builder, string text, string className = null)
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