namespace Known.Blazor;

/// <summary>
/// 后台布局模板组件类。
/// </summary>
partial class AntLayout
{
    private string spinTip = "";
    private bool showSpin = false;
    private bool collapsed = false;
    private bool showSetting = false;
    private string MenuClass => Context.UserSetting.MenuTheme == "Dark" ? "kui-menu-dark" : "";
    private UserSettingInfo Setting { get; set; } = new();

    [CascadingParameter] private RouteData RouteData { get; set; }

    /// <summary>
    /// 取得是否首次加载页面。
    /// </summary>
    protected bool IsLoaded { get; private set; }

    /// <summary>
    /// 取得后台管理主页数据对象。
    /// </summary>
    protected AdminInfo Info { get; private set; }

    /// <summary>
    /// 取得当前用户权限菜单列表。
    /// </summary>
    protected List<MenuInfo> UserMenus { get; private set; }

    /// <summary>
    /// 取得用户是否已经登录。
    /// </summary>
    protected bool IsLogin { get; private set; }

    /// <summary>
    /// 异步显示快速旋转加载提示。
    /// </summary>
    /// <param name="text">提示文本。</param>
    /// <param name="action">耗时操作委托。</param>
    /// <returns></returns>
    public override async Task ShowSpinAsync(string text, Func<Task> action)
    {
        if (action == null)
            return;

        showSpin = true;
        spinTip = text;
        await StateChangedAsync();
        await Task.Run(async () =>
        {
            try
            {
                await action?.Invoke();
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
            showSpin = false;
            await StateChangedAsync();
        });
    }

    /// <summary>
    /// 异步初始化模板。
    /// 如果系统未安装，则跳转到安装页面；
    /// 如果系统未登录，则跳转到登录页面。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitAsync()
    {
        IsLoaded = false;
        await base.OnInitAsync();
        if (Context.System == null)
        {
            Navigation?.NavigateTo("/install", true);
        }
        else
        {
            var user = await GetCurrentUserAsync();
            IsLogin = user != null;
            if (IsLogin)
            {
                Context.CurrentUser = user;
                Info = await Auth.GetAdminAsync();
                Context.UserSetting = Info?.UserSetting ?? new();
                Context.UserTableSettings = Info?.UserTableSettings ?? [];
                if (!Context.IsMobileApp)
                    UserMenus = GetUserMenus(Info?.UserMenus);
                Cache.AttachCodes(Info?.Codes);
                Setting = Context.UserSetting;
                await OnThemeColorAsync();
                IsLoaded = true;
            }
            else
            {
                Navigation?.GoLoginPage();
            }
        }
    }

    /// <summary>
    /// 异步设置模板页参数。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        try
        {
            var url = Navigation.GetPageUrl();
            var pageRoute = url.StartsWith("/page/") ? url.Substring(6) : "";
            Context.Url = url;
            Context.SetCurrentMenu(RouteData, pageRoute);
            if (!UIConfig.IgnoreRoutes.Contains(url) && !RouteData.PageType.IsAllowAnonymous())
            {
                if (Context.Current == null)
                {
                    Navigation.GoErrorPage("403");
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            OnError(ex);
        }
    }

    /// <summary>
    /// 异步获取第三方用户登录虚方法。
    /// </summary>
    /// <returns>用户信息。</returns>
    protected virtual Task<UserInfo> GetThirdUserAsync()
    {
        UserInfo user = null;
        return Task.FromResult(user);
    }

    /// <summary>
    /// 异步获取当前登录用户信息。
    /// </summary>
    /// <returns>用户信息。</returns>
    protected virtual async Task<UserInfo> GetCurrentUserAsync()
    {
        if (AuthProvider != null)
        {
            var user = await AuthProvider.GetUserAsync();
            if (user != null)
                return user;
        }

        return await GetThirdUserAsync();
    }

    private List<MenuInfo> GetUserMenus(List<MenuInfo> menus)
    {
        Context.UserMenus = menus;
        return menus.ToMenuItems();
    }

    private void OnToggle() => collapsed = !collapsed;

    private async Task OnThemeColorAsync()
    {
        var theme = Setting.ThemeColor;
        var href = $"_content/Known/css/theme/{theme}.css";
        await JS.SetStyleSheetAsync("/theme/", href);
    }

    private async Task OnSaveSetting()
    {
        var result = await System.SaveUserSettingInfoAsync(Setting);
        if (result.IsValid)
        {
            Context.UserSetting = Setting;
            await OnThemeColorAsync();
        }
    }

    private Task OnResetSetting()
    {
        Setting = new();
        return OnSaveSetting();
    }

    private void OnMenuClick(string id)
    {
        switch (id)
        {
            case "logout":
                UI?.Confirm(Language["Tip.Exits"], SignOutAsync);
                break;
            case "setting":
                showSetting = true;
                StateHasChanged();
                break;
        }
    }
}