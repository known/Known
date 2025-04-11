namespace Known.Internals;

class NavRenderMode : BaseNav
{
    protected override string Title => "呈现模式";
    protected override string Icon => "control";

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Dropdown(new DropdownModel
        {
            Icon = Icon,
            Items =
            [
                new() { Id = "Auto", Name = "自动模式" },
                new() { Id = "Server", Name = "SSR模式" }
            ],
            OnItemClick = OnItemClickAsync
        });
    }

    private async Task OnItemClickAsync(ActionInfo info)
    {
        await Platform.SetRenderModeAsync(info.Id);
        Navigation.NavigateTo("/", true);
    }
}