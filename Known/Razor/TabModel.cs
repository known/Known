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
    public ItemModel Current { get; set; }
    public bool IsContent { get; set; }
    public bool IsView { get; set; }
    public Func<bool, Task<bool>> OnSave { get; set; }

    public int GetCurrentIndex()
    {
        if (Current == null)
            return 0;

        return Items.IndexOf(Current);
    }
}

public class ItemModel
{
    public ItemModel(string title)
    {
        Title = title;
    }

    public string Title { get; }
    public string SubTitle { get; set; }
    public string Description { get; set; }
    public RenderFragment Content { get; set; }
}