namespace Known.Blazor;

/// <summary>
/// 基础模板组件类。
/// </summary>
public class BaseLayout : BaseComponent
{
    private AdminInfo Info { get; set; }
    internal MenuInfo CurrentMenu => Context?.Current;

    [CascadingParameter] private RouteData RouteData { get; set; }
    [Inject] private IAuthStateProvider AuthProvider { get; set; }

    /// <summary>
    /// 取得或设置是否是管理后台模板。
    /// </summary>
    [Parameter] public bool IsAdmin { get; set; }

    /// <summary>
    /// 取得当前用户权限菜单列表。
    /// </summary>
    [Parameter] public List<MenuInfo> UserMenus { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        if (!IsAdmin)
            return;

        await base.OnInitAsync();
        if (IsServerMode)
            await InitAdminAsync();
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
        if (firstRender)
        {
            await OnThemeColorAsync();
            if (Config.App.IsSize)
            {
                var size = await JS.GetCurrentSizeAsync();
                if (string.IsNullOrWhiteSpace(size))
                    size = Context.UserSetting.Size;
                if (string.IsNullOrWhiteSpace(size))
                    size = Config.App.DefaultSize;
                await JS.SetCurrentSizeAsync(size);
            }
            if (Config.App.IsLanguage)
            {
                var language = await JS.GetCurrentLanguageAsync();
                if (string.IsNullOrWhiteSpace(language))
                    language = Context.UserSetting.Language;
                Context.CurrentLanguage = language;
            }

            if (!IsServerMode)
            {
                await InitAdminAsync();
                LoadMenus();
            }
        }
    }

    /// <summary>
    /// 导航到指定菜单对应的页面。
    /// </summary>
    /// <param name="item">跳转的菜单对象。</param>
    public void NavigateTo(MenuInfo item) => Navigation?.NavigateTo(item);

    /// <summary>
    /// 返回到上一个页面。
    /// </summary>
    public void Back()
    {
        if (CurrentMenu == null || string.IsNullOrWhiteSpace(CurrentMenu.BackUrl))
            return;

        Navigation?.NavigateTo(CurrentMenu.BackUrl);
    }

    /// <summary>
    /// 异步显示加载提示框。
    /// </summary>
    /// <param name="text">加载提示信息。</param>
    /// <param name="action">异步加载方法的委托。</param>
    /// <returns></returns>
    public virtual Task ShowSpinAsync(string text, Func<Task> action) => Task.CompletedTask;

    /// <summary>
    /// 异步设置当前登录用户信息。
    /// </summary>
    /// <param name="user">用户信息。</param>
    /// <returns></returns>
    public async Task SignInAsync(UserInfo user)
    {
        if (AuthProvider != null)
            await AuthProvider.SignInAsync(user);
    }

    /// <summary>
    /// 异步注销，用户安全退出系统。
    /// </summary>
    /// <returns></returns>
    public async Task SignOutAsync()
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

    /// <summary>
    /// 重新加载当前页面，如果是多标签，则刷新当前标签页。
    /// </summary>
    public virtual void ReloadPage() { }

    internal void Logout()
    {
        UI.Confirm(Language["Tip.Exits"], SignOutAsync);
    }

    internal virtual void LoadMenus() { }

    internal virtual void AddMenuItem(MenuInfo item)
    {
        if (Context.UserMenus == null)
            Context.UserMenus = [];
        Context.UserMenus.Add(item);
        UserMenus = Context.UserMenus.ToMenuItems();
        LoadMenus();
    }

    internal async Task OnThemeColorAsync()
    {
        var theme = Context.UserSetting.ThemeColor;
        var href = $"_content/Known/css/theme/{theme}.css";
        await JS.SetStyleSheetAsync("/theme/", href);
    }

    private async Task InitAdminAsync()
    {
        Info = await Admin.GetAdminAsync();
        Context.UserSetting = Info?.UserSetting ?? new();
        Context.UserTableSettings = Info?.UserTableSettings ?? [];
        if (!Context.IsMobileApp)
            SetUserMenus(Info?.UserMenus);
        Cache.AttachCodes(Info?.Codes);
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
}