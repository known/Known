namespace Known.Internals;

/// <summary>
/// 顶部用户导航组件类。
/// </summary>
public partial class NavUser
{
    private readonly DropdownModel model = new();
    private string AvatarUrl => CurrentUser?.AvatarUrl ?? "img/face1.png";

    [CascadingParameter] private TopNavbar Topbar { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        model.Items =
        [
            new ActionInfo { Id = "profile", Name = Language.Profile, Icon = "user", Url = "/profile" },
            new ActionInfo { Id = "logout", Name = Language.Exit, Icon = "poweroff" },
        ];
        model.OnItemClick = OnItemClickAsync;
    }

    private Task OnItemClickAsync(ActionInfo item)
    {
        if (!string.IsNullOrWhiteSpace(item.Url))
            Context.NavigateTo(item);
        else if (item.Id == "logout")
            App?.Logout();
        Topbar?.OnActionClick?.Invoke(item);
        return Task.CompletedTask;
    }
}