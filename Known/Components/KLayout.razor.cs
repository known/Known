using AntDesign;

namespace Known.Components;

/// <summary>
/// 后台布局模板组件类。
/// </summary>
partial class KLayout
{
    private string spinTip = "";
    private bool showSpin = false;
    private bool showSetting = false;
    private bool collapsed = false;

    private MainMenu menu;
    private ReloadContainer reload;

    private string WrapperClass => CssBuilder.Default("kui-wrapper").AddClass(Setting.Size).BuildClass();
    private string TabsClass => CssBuilder.Default("kui-nav-tabs").AddClass("is-top", Setting.IsTopTab).BuildClass();
    private string HeaderClass => CssBuilder.Default("kui-header")
                                            .AddClass("kui-menu-dark", Setting.MenuTheme == "Dark")
                                            .BuildClass();
    private string MenuClass => CssBuilder.Default()
                                          .AddClass("kui-menu-dark", Setting.MenuTheme == "Dark")
                                          .AddClass("kui-menu-float", Setting.LayoutMode == LayoutMode.Float.ToString())
                                          .BuildClass();

    private AdminInfo Info { get; set; }
    [Inject] private IAuthStateProvider AuthProvider { get; set; }
    [Inject] private ReuseTabsService Service { get; set; }

    /// <summary>
    /// 取得或设置页面是否加载完成。
    /// </summary>
    protected bool IsLoaded { get; set; } = true;

    /// <summary>
    /// 取得或设置用户设置信息。
    /// </summary>
    protected UserSettingInfo Setting { get; set; } = new();

    /// <summary>
    /// 取得或设置路由数据对象。
    /// </summary>
    [CascadingParameter] protected RouteData RouteData { get; set; }

    /// <summary>
    /// 取得或设置是否是管理后台模板。
    /// </summary>
    [Parameter] public bool IsAdmin { get; set; }

    /// <summary>
    /// 取得当前用户权限菜单列表。
    /// </summary>
    [Parameter] public List<MenuInfo> UserMenus { get; set; }

    /// <summary>
    /// 取得或设置子组件内容。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <inheritdoc />
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
                await OnErrorAsync(ex);
            }
            showSpin = false;
            await StateChangedAsync();
        });
    }

    /// <inheritdoc />
    public override async Task SignInAsync(UserInfo user)
    {
        if (AuthProvider != null)
            await AuthProvider.SignInAsync(user);
    }

    /// <inheritdoc />
    public override async Task SignOutAsync()
    {
        var result = await Admin.SignOutAsync();
        if (result.IsValid)
        {
            Context.SignOut();
            await SignInAsync(null);
            Navigation?.GoLoginPage();
            Config.OnExit?.Invoke();
        }
    }

    /// <inheritdoc />
    public override void ReloadPage()
    {
        if (Setting.MultiTab)
            Service.ReloadPage();
        else
            reload?.Reload();
    }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        if (!IsAdmin)
            return;

        IsLoaded = false;
        await base.OnInitAsync();
        if (!Config.IsInstalled)
        {
            var isInstall = await Admin.GetIsInstallAsync(); //检查是否需要安装
            Config.IsInstalled = !isInstall;
        }
        if (!Config.IsInstalled)
        {
            Navigation?.GoInstallPage();
            return;
        }

        var user = await GetCurrentUserAsync();
        if (user == null)
        {
            Navigation?.GoLoginPage();
            return;
        }

        Context.CurrentUser = user;
        Info = await Admin.GetAdminAsync();
        Context.UserSetting = Info?.UserSetting ?? new();
        Context.UserTableSettings = Info?.UserTableSettings ?? [];
        if (!Context.IsMobileApp)
            SetUserMenus(Info?.UserMenus);
        Cache.AttachCodes(Info?.Codes);
        Setting = Context.UserSetting;
        IsLoaded = true;
    }

    /// <inheritdoc />
    protected override async Task OnParameterAsync()
    {
        await base.OnParameterAsync();
        Context.Url = Navigation.GetPageUrl();
        Context.SetCurrentMenu(RouteData);
        CheckUrlAuthentication();
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        try
        {
            if (firstRender)
            {
                await JS.InitFilesAsync();
                await OnThemeColorAsync();
                if (Config.App.IsSize)
                {
                    var size = await JS.GetCurrentSizeAsync();
                    if (string.IsNullOrWhiteSpace(size))
                        size = Setting.Size;
                    if (string.IsNullOrWhiteSpace(size))
                        size = Config.App.DefaultSize;
                    await JS.SetCurrentSizeAsync(size);
                }
                if (Config.App.IsLanguage)
                {
                    var language = await JS.GetCurrentLanguageAsync();
                    if (string.IsNullOrWhiteSpace(language))
                        language = Setting.Language;
                    Context.CurrentLanguage = language;
                }
            }
            menu?.SetItems(UserMenus);
        }
        catch (Exception ex)
        {
            await OnErrorAsync(ex);
        }
    }

    internal override void ToggleSide(bool collapsed)
    {
        this.collapsed = collapsed;
        StateChanged();
    }

    internal override void AddMenuItem(MenuInfo item)
    {
        if (Context.UserMenus == null)
            Context.UserMenus = [];
        Context.UserMenus.Add(item);
        UserMenus = Context.UserMenus.ToMenuItems();
        menu?.SetItems(UserMenus);
    }

    private async Task<UserInfo> GetCurrentUserAsync()
    {
        if (AuthProvider == null)
            return null;

        return await AuthProvider.GetUserAsync();
    }

    private void SetUserMenus(List<MenuInfo> menus)
    {
        Context.UserMenus = menus;
        UserMenus = menus?.ToMenuItems();
    }

    private void CheckUrlAuthentication()
    {
        if (!IsAdmin)
            return;

        if (!UIConfig.IgnoreRoutes.Contains(Context.Url) && !RouteData.PageType.IsAllowAnonymous())
        {
            if (Context.Current == null)
            {
                Navigation.GoErrorPage("403");
                return;
            }
        }
    }

    private async Task OnThemeColorAsync()
    {
        var theme = Setting.ThemeColor;
        var href = $"_content/Known/css/theme/{theme}.css";
        await JS.SetStyleSheetAsync("/theme/", href);
    }

    private async Task OnSaveSetting()
    {
        var result = await Admin.SaveUserSettingAsync(Setting);
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

    private void OnLogoClick()
    {
        Navigation.NavigateTo("/");
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