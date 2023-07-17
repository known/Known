using Known.Razor.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using WebSite.Data;

namespace WebSite.Docus;

class BaseDocu : ComponentBase
{
    [Parameter] public MenuItem? Item { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Element("h1", attr => builder.Text($"{Item?.Name} ({Item?.Id})"));
        builder.Div("doc-desc", Item?.Description);
    }
}