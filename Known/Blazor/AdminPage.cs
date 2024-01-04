using Known.Designers;
using Known.Extensions;
using Microsoft.AspNetCore.Components;

namespace Known.Blazor;

public class AdminPage : BaseComponent
{
    public AdminPage()
    {
        Context.OnNavigate = OnNavigate;
        Context.OnRefreshPage = RefreshPage;
        TabMenus = [Config.GetHomeMenu()];
    }

    [Parameter] public Action OnLogout { get; set; }

    protected AdminInfo Info { get; private set; }
    protected List<MenuItem> UserMenus { get; private set; }
    protected List<MenuItem> TabMenus { get; }
    protected MenuItem CurrentMenu { get; private set; }

    protected virtual void RefreshPage() => StateChanged();

    protected override async Task OnInitializedAsync()
    {
        IsLoaded = false;
        await DataHelper.InitializeAsync(Platform.Module);
        CurrentMenu = Config.GetHomeMenu();
        Info = await Platform.Auth.GetAdminAsync();
        UserMenus = GetUserMenus(Info?.UserMenus);
        Context.UserSetting = Info?.UserSetting ?? new();
        IsLoaded = true;
    }

    protected async Task SignOutAsync()
    {
        var user = CurrentUser;
        var result = await Platform.Auth.SignOutAsync(user?.Token);
        if (result.IsValid)
        {
            OnLogout?.Invoke();
            Config.OnExit?.Invoke();
        }
    }

    private void OnNavigate(MenuItem item)
    {
        item.ComType = Config.PageTypes.GetValueOrDefault(item.Code);
        if (item.ComType == null)
            item.ComType = typeof(BasePage);
        item.ComParameters = new Dictionary<string, object> { [nameof(BasePage.PageId)] = item.Id };
        CurrentMenu = item;

        if (Context.UserSetting.MultiTab)
        {
            if (!TabMenus.Exists(m => m.Id == item.Id))
                TabMenus.Add(item);
        }

        StateChanged();
    }

    private List<MenuItem> GetUserMenus(List<MenuInfo> menus)
    {
        Context.UserMenus = menus;
        return menus.ToMenuItems();
    }
}