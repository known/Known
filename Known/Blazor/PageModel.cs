using Microsoft.AspNetCore.Components;

namespace Known.Blazor;

public class PageModel
{
	public PageType Type { get; set; }
    public string Spans { get; set; }
    public List<PageItemModel> Items { get; } = [];
	public Action StateChanged { get; set; }

    public void AddItem(RenderFragment content)
    {
        Items.Add(new PageItemModel { Content = content });
    }

    public void AddItem(string className, RenderFragment content)
    {
        Items.Add(new PageItemModel { ClassName = className, Content = content });
    }
}

public class PageItemModel
{
    public string ClassName { get; set; }
    public RenderFragment Content { get; set; }
}