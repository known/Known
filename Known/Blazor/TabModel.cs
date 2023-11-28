using Microsoft.AspNetCore.Components;

namespace Known.Blazor;

public class TabModel
{
    public List<ItemModel> Items { get; } = [];
}

public class StepModel
{
    public List<ItemModel> Items { get; } = [];
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

public class ItemModel(string title)
{
    public string Title { get; } = title;
    public string SubTitle { get; set; }
    public string Description { get; set; }
    public RenderFragment Content { get; set; }
}