using Microsoft.AspNetCore.Components;

namespace Known.Razor;

public class TabModel
{
    public TabModel()
    {
        Tabs = [];
    }

    public List<ItemModel> Tabs { get; }
}

public class ItemModel
{
    public string Title { get; set; }
    public RenderFragment Content { get; set; }
}