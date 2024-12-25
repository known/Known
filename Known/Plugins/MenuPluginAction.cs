namespace Known.Plugins;

class MenuPluginAction : BaseComponent
{
    private List<ActionInfo> items = [];

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
            TriggerType = "Click",
            Text = "添加菜单项",
            OnItemClick = OnMenuClickAsync
        };
        builder.Dropdown(model);
    }

    private static List<ActionInfo> GetActionItems()
    {
        var plugins = Config.Plugins.Where(p => p.IsMenu).ToList();
        return plugins.ToActions();
    }

    private Task OnMenuClickAsync(ActionInfo info)
    {
        UI.Alert(info.Name);
        return Task.CompletedTask;
    }
}