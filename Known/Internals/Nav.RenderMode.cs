namespace Known.Internals;

class NavRenderMode : BaseNav
{
    protected override string Title => "呈现模式";
    protected override string Icon => IsServerMode ? "control" : "project";

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
        if (Config.RenderMode != RenderType.Auto && info.Id == "Auto")
        {
            UI.Error("当前程序不支持切换为自动模式。");
            return;
        }

        await Platform.SetRenderModeAsync(info.Id);
        Navigation.NavigateTo("/", true);
    }
}