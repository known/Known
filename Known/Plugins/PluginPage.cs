namespace Known.Plugins;

class PluginPage : BasePage, IAutoPage
{
    private List<ActionInfo> items = [];

    [Parameter] public MenuInfo Menu { get; set; }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        items = GetActionItems();
    }

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        if (Menu.Plugins != null && Menu.Plugins.Count > 0)
        {
            builder.Cascading(this, b =>
            {
                foreach (var item in Menu.Plugins)
                {
                    b.BuildPlugin(item);
                }
            });
        }

        if (UIConfig.IsEditMode)
            BuildAction(builder);
    }

    private void BuildAction(RenderTreeBuilder builder)
    {
        var model = new DropdownModel
        {
            Class = "kui-edit",
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

    private async Task OnPageClickAsync(ActionInfo info)
    {
        // 向当前页面添加插件实例
        Menu.Plugins.Add(new PluginInfo
        {
            Id = Utils.GetGuid(),
            Type = info.Id
        });
        await Platform.SaveMenuAsync(Menu);
        await StateChangedAsync();
    }
}