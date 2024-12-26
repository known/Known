namespace Known.Plugins;

class PluginPage : BaseComponent, IAutoPage
{
    private List<ActionInfo> items = [];

    [Parameter] public MenuInfo Menu { get; set; }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        items = GetActionItems();
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (Menu.Plugins != null && Menu.Plugins.Count > 0)
        {
            foreach (var item in Menu.Plugins)
            {
                var plugin = Config.Plugins.FirstOrDefault(p => p.Id == item.Id);
                if (plugin != null)
                {
                    plugin.Parameter = item.Setting;
                    builder.BuildPlugin(this, plugin);
                }
            }
        }

        if (UIConfig.IsEditMode)
            BuildAction(builder);
    }

    private void BuildAction(RenderTreeBuilder builder)
    {
        var model = new DropdownModel
        {
            Icon = "plus",
            Items = items,
            TriggerType = "Click",
            Text = "添加区块",
            OnItemClick = OnPageClickAsync
        };
        builder.Dropdown(model);
    }

    private static List<ActionInfo> GetActionItems()
    {
        var plugins = Config.Plugins.Where(p => p.IsPage).ToList();
        return plugins.ToActions();
    }

    private Task OnPageClickAsync(ActionInfo info)
    {
        UI.Alert(info.Name);
        return Task.CompletedTask;
    }
}