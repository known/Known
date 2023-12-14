using Microsoft.AspNetCore.Components;

namespace Known.Blazor;

public class ItemModel(string title)
{
    public string Title { get; } = title;
    public string SubTitle { get; set; }
    public string Description { get; set; }
    public RenderFragment Content { get; set; }
}

public class TabModel
{
    public List<ItemModel> Items { get; } = [];
}

public class StepModel
{
    public int Current { get; set; }
    public List<ItemModel> Items { get; } = [];
}