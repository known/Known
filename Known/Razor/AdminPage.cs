﻿using Known.Extensions;
using Microsoft.AspNetCore.Components;

namespace Known.Razor;

public class AdminPage : BaseComponent
{
    public AdminPage()
    {
        Context.OnNavigate = OnNavigate;
    }

    [Parameter] public Action OnLogout { get; set; }

    protected AdminInfo Info { get; private set; }
    protected List<MenuItem> UserMenus { get; private set; }
    protected MenuItem CurrentMenu { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        IsLoaded = false;
        CurrentMenu = Config.GetHomeMenu();
        Info = await Platform.GetAdminAsync();
        UserMenus = GetUserMenus(Info?.UserMenus);
        //Setting.UserSetting = info?.UserSetting;
        //Setting.Info = info?.UserSetting?.Info ?? SettingInfo.Default;

        if (Config.IsWebApi)
            Cache.AttachCodes(Info?.Codes);

        //if (TopMenu)
        //{
        //    topMenu = userMenus.FirstOrDefault();
        //    curMenu = topMenu.Children.FirstOrDefault();
        //}

        //PageAction.RefreshTheme = () =>
        //{
        //    UI.CurDialog = UI.TopDialog;
        //    StateChanged();
        //};
        IsLoaded = true;
    }

    protected void OnGoHome() => Context.NavigateToHome();

    protected Task<Result> SignOutAsync()
    {
        var user = CurrentUser;
        return Platform.User.SignOutAsync(user?.Token);
    }

    private void OnNavigate(MenuItem item)
    {
        item.ComType = Config.PageTypes.GetValue(item.Code);
        if (item.ComType == null)
            item.ComType = typeof(BasePage);
        item.ComParameters = new Dictionary<string, object> { [nameof(BasePage.PageId)] = item.Id };
        CurrentMenu = item;
        StateChanged();
    }

    private List<MenuItem> GetUserMenus(List<MenuInfo> menus)
    {
        Context.UserMenus = menus;
        return menus.ToMenuItems();
    }
}