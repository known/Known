using Microsoft.AspNetCore.Components;

namespace KnownAntDesign;

class KaConfig
{
    public static AntDesignOption Option { get; set; }
}

class StepItem
{
    public string Title { get; set; }
    public RenderFragment Content { get; set; }
}