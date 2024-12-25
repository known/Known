namespace Known.Plugins;

class PagePluginAction : BaseComponent
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