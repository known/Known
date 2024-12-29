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
        if (Menu != null && Menu.Plugins != null && Menu.Plugins.Count > 0)
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
            OnItemClick = OnItemClickAsync
        };
        builder.Dropdown(model);
    }

    private static List<ActionInfo> GetActionItems()
    {
        var plugins = Config.Plugins.Where(p => p.IsPage).ToList();
        return plugins.ToActions();
    }

    private async Task OnItemClickAsync(ActionInfo info)
    {
        if (Menu == null)
        {
            UI.Error("菜单不存在！");
            return;
        }

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