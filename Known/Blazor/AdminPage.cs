using Known.Extensions;
using Microsoft.AspNetCore.Components;

namespace Known.Blazor;

public class AdminPage : BaseComponent
{
    public AdminPage()
    {
        Context.OnNavigate = OnNavigate;
        Context.OnRefreshPage = StateChanged;
        TabMenus = [Config.GetHomeMenu()];
    }

    [Parameter] public Action OnLogout { get; set; }

    protected AdminInfo Info { get; private set; }
    protected List<MenuItem> UserMenus { get; private set; }
    protected List<MenuItem> TabMenus { get; }
    protected MenuItem CurrentMenu { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        IsLoaded = false;
        CurrentMenu = Config.GetHomeMenu();
        Info = await Platform.Auth.GetAdminAsync();
        UserMenus = GetUserMenus(Info?.UserMenus);
        Context.UserSetting = Info?.UserSetting ?? new();
        IsLoaded = true;
    }

    protected Task<Result> SignOutAsync()
    {
        var user = CurrentUser;
        return Platform.Auth.SignOutAsync(user?.Token);
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