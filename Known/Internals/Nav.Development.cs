namespace Known.Internals;

class NavDevelopment : BaseNav
{
    private List<ActionInfo> items = [];

    [CascadingParameter] private TopNavbar Topbar { get; set; }

    protected override string Title => Language.Development;
    protected override string Icon => "appstore";

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        var plugins = PluginConfig.DevPlugins;
        if (!UIConfig.EnableEdit)
            plugins = [.. plugins.Where(p => p.Type != typeof(LanguagePage) && p.Type != typeof(ButtonPage))];
        if (!Config.App.IsLanguage)
            plugins = [.. plugins.Where(p => p.Type != typeof(LanguagePage))];
        if (!Config.IsDebug)
            plugins = [.. plugins.Where(p => p.Type != typeof(CodingPage))];
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
            Context.NavigateTo(item);
        Topbar?.OnActionClick?.Invoke(item);
        return Task.CompletedTask;
    }
}