using Microsoft.AspNetCore.Components;

namespace Known.Razor;

public class TabModel
{
    public TabModel()
    {
        Items = [];
    }

    public List<ItemModel> Items { get; }
}

public class StepModel
{
    public StepModel()
    {
        Items = [];
    }

    public List<ItemModel> Items { get; }
    public bool IsContent { get; set; }
    public bool IsView { get; set; }
    public Func<bool, Task<bool>> OnSave { get; set; }
}

public class ItemModel
{
    public ItemModel(string title)
    {
        Title = title;
    }

    public string Title { get; }
    public RenderFragment Content { get; set; }
}