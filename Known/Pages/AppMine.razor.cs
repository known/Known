namespace Known.Pages;

/// <summary>
/// 移动端我的页面组件类。
/// </summary>
public partial class AppMine
{
    private UserInfo user;
    private readonly List<MenuInfo> items = [];

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        user = CurrentUser;
        items.Add(new MenuInfo { Id = "Mobile", Name = user?.Mobile, Icon = "mobile" });
        items.Add(new MenuInfo { Id = "Email", Name = user?.Email, Icon = "inbox" });
        items.Add(new MenuInfo
        {
            Id = "Profile",
            Name = Language.GetString("Title.MyProfile"),
            Icon = "user",
            Url = "/profile/user",
            BackUrl = "/app/mine"
        });
        items.Add(new MenuInfo
        {
            Id = "Password",
            Name = Language.GetString("Title.SecuritySetting"),
            Icon = "lock",
            Url = "/profile/password",
            BackUrl = "/app/mine"
        });
    }

    private void OnItemClick(MenuInfo item)
    {
        if (string.IsNullOrWhiteSpace(item.Url))
            return;

        Context.NavigateTo(item);
    }

    private void OnExit(MouseEventArgs args)
    {
        App.Logout();
    }
}