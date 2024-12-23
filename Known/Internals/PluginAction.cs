namespace Known.Internals;

class PluginAction : BaseComponent
{
    private List<ActionInfo> items = [];

    [Parameter] public PluginType Type { get; set; }
    [Parameter] public List<string> Values { get; set; }
    [Parameter] public Func<ActionInfo, Task> OnAdded { get; set; }

    internal void SetValues(List<string> values)
    {
        Values = values;
        items = GetActionItems();
        StateChanged();
    }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        items = GetActionItems();
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        var model = new DropdownModel
        {
            Icon = "plus",
            Items = items,
            TriggerType = "Click"
        };
        if (Type == PluginType.Navbar)
        {
            model.Tooltip = "添加导航";
            model.OnItemClick = OnNavbarClickAsync;
            builder.Component<AntDropdown>()
                   .Set(c => c.Model, model)
                   .Set(c => c.Placement, AntDesign.Placement.BottomRight)
                   .Build();
        }
        else if (Type == PluginType.Menu)
        {
            model.Text = "添加菜单项";
            model.OnItemClick = OnMenuClickAsync;
            builder.Dropdown(model);
        }
        else if (Type == PluginType.Page)
        {
            model.Text = "添加区块";
            model.OnItemClick = OnPageClickAsync;
            builder.Dropdown(model);
        }
    }

    private List<ActionInfo> GetActionItems()
    {
        var plugins = Config.Plugins.Where(p => p.Type == Type && !Values.Contains(p.Id)).ToList();
        var infos = new List<ActionInfo>();
        var categories = plugins.Select(p => p.Category).Distinct();
        foreach (var category in categories)
        {
            var items = plugins.Where(p => p.Category == category);
            var info = new ActionInfo
            {
                Id = category,
                Icon = "folder",
                Name = category
            };
            info.Children.AddRange(items.Select(GetAction));
            infos.Add(info);
        }
        var others = plugins.Where(p => string.IsNullOrWhiteSpace(p.Category));
        foreach (var item in others)
        {
            infos.Add(GetAction(item));
        }
        return infos;
    }

    private static ActionInfo GetAction(PluginAttribute item)
    {
        return new ActionInfo
        {
            Id = item.Id,
            Name = item.Name,
            Icon = item.Icon ?? "file"
        };
    }

    private async Task OnNavbarClickAsync(ActionInfo info)
    {
        var plugin = Config.Plugins.FirstOrDefault(p => p.Id == info.Id);
        if (plugin != null)
            await OnAdded?.Invoke(info);
    }

    private Task OnMenuClickAsync(ActionInfo info)
    {
        UI.Alert(info.Name);
        return Task.CompletedTask;
    }

    private Task OnPageClickAsync(ActionInfo info)
    {
        UI.Alert(info.Name);
        return Task.CompletedTask;
    }
}