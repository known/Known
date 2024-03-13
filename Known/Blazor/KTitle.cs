using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class KTitle : BaseComponent
{
    [Parameter] public string Text { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (!string.IsNullOrWhiteSpace(Text))
            builder.Div("kui-title", Text);
        else
            builder.Div("kui-title", () => ChildContent(builder));
    }
}