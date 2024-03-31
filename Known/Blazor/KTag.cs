using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class KTag : BaseComponent
{
    [Parameter] public string Text { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        UI.BuildTag(builder, Text);
    }
}