using Microsoft.AspNetCore.Components;

namespace KnownAntDesign;

class StepItem
{
    public string Title { get; set; }
    public RenderFragment Content { get; set; }
}