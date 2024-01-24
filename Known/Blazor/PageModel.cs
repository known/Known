using Microsoft.AspNetCore.Components;

namespace Known.Blazor;

public class PageModel
{
	public PageType Type { get; set; }
    public string Spans { get; set; }
    public List<RenderFragment> Contents { get; set; } = [];
	public Action StateChanged { get; set; }
}