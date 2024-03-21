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
    public RenderFragment Left { get; set; }
    public RenderFragment Right { get; set; }
    public List<ItemModel> Items { get; } = [];
    public Action<string> OnChange { get; set; }

    public bool HasItem => Items != null && Items.Count > 0;

    public void AddTab(string title)
    {
        Items.Add(new ItemModel(title));
    }

    public void AddTab(string title, RenderFragment content)
    {
        Items.Add(new ItemModel(title) { Content = content });
    }
}

public class StepModel
{
    public string Direction { get; set; }
    public int Current { get; set; }
    public List<ItemModel> Items { get; } = [];

    public void AddStep(string title, RenderFragment content)
    {
        Items.Add(new ItemModel(title) { Content = content });
    }
}

public class ToolbarModel
{
    public int ShowCount { get; set; } = 4;
    public List<ActionInfo> Items { get; set; } = [];
    public Action<ActionInfo> OnItemClick { get; set; }
    public Action OnRefresh { get; set; }

    public bool HasItem => Items != null && Items.Count > 0;

    public void AddAction(string idOrName) => Items.Add(new ActionInfo(idOrName));
    public void Refresh() => OnRefresh.Invoke();
}