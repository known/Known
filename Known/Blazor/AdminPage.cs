using Known.Extensions;
using Microsoft.AspNetCore.Components;

namespace Known.Blazor;

public class AdminPage : BaseComponent
{
    public AdminPage()
    {
        TabMenus = [Config.GetHomeMenu()];
    }

    [Parameter] public Func<Task> OnLogout { get; set; }

    protected AdminInfo Info { get; private set; }
    protected List<MenuItem> UserMenus { get; private set; }
    protected List<MenuItem> TabMenus { get; }
    protected MenuItem CurrentMenu { get; private set; }

    public virtual void SetCurrentMenu(MenuItem item)
    {
        CurrentMenu = item;
        StateChanged();
    }

    public virtual Task ShowSpinAsync(string text = null) => Task.CompletedTask;
    public virtual void HideSpin() { }

    protected virtual void RefreshPage() => StateChanged();

    protected override async Task OnInitAsync()
    {
        IsLoaded = false;
        await base.OnInitAsync();
        Context.OnNavigate = OnNavigate;
        Context.OnRefreshPage = RefreshPage;
        CurrentMenu = Config.GetHomeMenu();
        Info = await Platform.Auth.GetAdminAsync();
        UserMenus = GetUserMenus(Info?.UserMenus);
        Context.UserSetting = Info?.UserSetting ?? new();
        IsLoaded = true;
    }

    protected async Task SignOutAsync()
    {
        var user = CurrentUser;
        var result = await Platform.SignOutAsync(user?.Token);
        if (result.IsValid)
        {
            await OnLogout?.Invoke();
            Config.OnExit?.Invoke();
        }
    }

    private bool isNavigating;
    private void OnNavigate(MenuItem item)
    {
        if (isNavigating || CurrentMenu == item)
            return;

        isNavigating = true;
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
        isNavigating = false;
    }

    private List<MenuItem> GetUserMenus(List<MenuInfo> menus)
    {
        Context.UserMenus = menus;
        return menus.ToMenuItems();
    }
}