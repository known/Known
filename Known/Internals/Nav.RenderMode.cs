namespace Known.Internals;

class NavRenderMode : BaseNav
{
    protected override string Title => Language.RenderMode;
    protected override string Icon => IsServerMode ? "control" : "project";

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Dropdown(new DropdownModel
        {
            Icon = Icon,
            Items =
            [
                new() { Id = "Auto", Icon = "project", Name = Language.AutoMode },
                new() { Id = "Server", Icon = "control", Name = Language.SSRMode }
            ],
            OnItemClick = OnItemClickAsync
        });
    }

    private async Task OnItemClickAsync(ActionInfo info)
    {
        if (Config.RenderMode != RenderType.Auto && info.Id == "Auto")
        {
            UI.Error(Language.NotSupportAutoMode);
            return;
        }

        var result = await Admin.SetRenderModeAsync(info.Id);
        UI.Result(result, () =>
        {
            Navigation.NavigateTo("/", true);
            return Task.CompletedTask;
        });
    }
}