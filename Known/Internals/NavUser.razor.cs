namespace Known.Internals;

/// <summary>
/// 顶部用户导航组件类。
/// </summary>
public partial class NavUser
{
    private readonly DropdownModel model = new();
    private string AvatarUrl => CurrentUser?.AvatarUrl ?? "img/face1.png";
    private string DisplayName => CurrentUser?.IsChangeTenant == true ? $"{CurrentUser?.Name}({CurrentUser?.CompName})" : CurrentUser?.Name;

    [CascadingParameter] private TopNavbar Topbar { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        model.Items = [];
        model.Items.Add(new ActionInfo { Id = "profile", Name = Language.Profile, Icon = "user", Url = "/profile" });
        if (Config.App.IsPlatform && CurrentUser.IsSystemAdmin())
        {
            model.Items.Add(new ActionInfo { Id = "switchTenant", Name = Language.SwitchTenant, Icon = "swap" });
        }
        model.Items.Add(new ActionInfo { Id = "logout", Name = Language.Exit, Icon = "poweroff" });
        model.OnItemClick = OnItemClickAsync;
    }

    private Task OnItemClickAsync(ActionInfo item)
    {
        if (!string.IsNullOrWhiteSpace(item.Url))
            Context.NavigateTo(item);
        else if (item.Id == "switchTenant")
            ShowTenantSwitch();
        else if (item.Id == "logout")
            App?.Logout();
        Topbar?.OnActionClick?.Invoke(item);
        return Task.CompletedTask;
    }

    private void ShowTenantSwitch()
    {
        var model = new DialogModel
        {
            Title = Language.SelectTenant,
            Width = 700,
            Content = b => b.Component<TenantSwitch>().Set(c => c.OnChange, StateChanged).Build()
        };
        UI.ShowDialog(model);
    }
}