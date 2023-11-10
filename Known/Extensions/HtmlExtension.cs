using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Extensions;

public static class HtmlExtension
{
    public static void Span(this RenderTreeBuilder builder, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        builder.OpenElement(0, "span");
        builder.AddContent(1, text);
        builder.CloseElement();
    }
}