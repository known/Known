namespace Known.Internals;

class NavDevelopment : BaseNav
{
    private List<ActionInfo> items = [];

    protected override string Title => Language["Nav.Development"];
    protected override string Icon => "appstore";

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        var plugins = Config.Plugins.Where(p => p.IsDev).ToList();
        items = plugins.ToActions();
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Dropdown(new DropdownModel
        {
            Icon = Icon,
            Items = items,
            OnItemClick = OnItemClickAsync
        });
    }

    private Task OnItemClickAsync(ActionInfo item)
    {
        if (!string.IsNullOrWhiteSpace(item.Url))
            Navigation?.NavigateTo(item);
        return Task.CompletedTask;
    }
}