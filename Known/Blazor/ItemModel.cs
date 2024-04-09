using Microsoft.AspNetCore.Components;

namespace Known.Blazor;

public class ItemModel(string id, string title)
{
    public string Id { get; } = id;
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

    public void AddTab(string id) => AddTab(id, id);

    public void AddTab(string id, string title)
    {
        Items.Add(new ItemModel(id, title));
    }

    public void AddTab(string id, RenderFragment content) => AddTab(id, id, content);

    public void AddTab(string id, string title, RenderFragment content)
    {
        Items.Add(new ItemModel(id, title) { Content = content });
    }
}

public class StepModel
{
    public string Class { get; set; }
    public string Direction { get; set; }
    public int Current { get; set; }
    public List<ItemModel> Items { get; } = [];

    public void AddStep(string title) => Items.Add(new ItemModel("", title));
    public void AddStep(string id, RenderFragment content) => AddStep(id, id, content);

    public void AddStep(string id, string title, RenderFragment content)
    {
        Items.Add(new ItemModel(id, title) { Content = content });
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
    public void Refresh() => OnRefresh?.Invoke();
}