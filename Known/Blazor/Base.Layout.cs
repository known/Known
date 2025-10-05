using AntDesign;

namespace Known.Blazor;

/// <summary>
/// 基础模板组件类。
/// </summary>
public class BaseLayout : BaseComponent
{
    private AdminInfo Info { get; set; }

    [CascadingParameter] private RouteData RouteData { get; set; }
    [Inject] private IAuthStateProvider AuthProvider { get; set; }
    [Inject] private ReuseTabsService TabsService { get; set; }

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
        await base.OnInitAsync();
        Context.App = this;
        Context.TabsService = TabsService;
        //if (IsServerMode)
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
            //if (!IsServerMode)
            //    await InitAdminAsync(); //App.ReloadPage();
            if (Info != null && Info.IsChangePwd)
                ShowUpdatePassword();
        }
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
            Navigation.GoLoginPage();
            Config.OnExit?.Invoke();
        }
    }

    /// <summary>
    /// 重新加载当前页面，如果是多标签，则刷新当前标签页。
    /// </summary>
    public virtual void ReloadPage() { }

    internal void Logout()
    {
        UI.Confirm(Language.TipExits, SignOutAsync);
    }

    internal virtual void LoadMenus() { }

    /// <summary>
    /// 添加菜单项。
    /// </summary>
    /// <param name="item">菜单项。</param>
    public virtual void AddMenuItem(MenuInfo item)
    {
        var menus = Context.UserMenus ?? [];
        menus.Add(item);
        SetUserMenus(menus);
    }

    /// <summary>
    /// 移除菜单项。
    /// </summary>
    /// <param name="item">菜单项。</param>
    public virtual void RemoveMenuItem(MenuInfo item)
    {
        var menus = Context.UserMenus ?? [];
        var info = menus.FirstOrDefault(m => m.Id == item.Id);
        if (info != null)
            menus.Remove(info);
        SetUserMenus(menus);
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
        Config.DatabaseType = Info.DatabaseType;
        if (Info.Actions != null)
            Config.Actions = [.. Info.Actions];
        Context.UserSetting = Info.UserSetting ?? new();
        Context.UserTableSettings = Info.UserTableSettings ?? [];
        if (!Context.IsMobileApp)
            SetUserMenus(Info.UserMenus);
        Cache.AttachCodes(Info.Codes);
        Config.OnAdmin?.Invoke(Info);
        UIConfig.Load(Info);
    }

    private void SetUserMenus(List<MenuInfo> menus)
    {
        Context.UserMenus = menus;
        UserMenus = menus?.Where(m => m.Target != Constants.Route).ToMenuItems();
        LoadMenus();
    }

    private void CheckUrlAuthentication()
    {
        if (Context.Url == "/") return;
        if (UIConfig.IgnoreRoutes.Contains(Context.Url)) return;
        if (RouteData.PageType.IsAllowAnonymous()) return;

        var roles = Context.Current?.Role?.Split(',');
        if (Context.Current == null || !CurrentUser.InRole(roles))
            Navigation.GoErrorPage("403");
    }

    private void ShowUpdatePassword()
    {
        DialogModel model = null;
        model = new DialogModel
        {
            Title = Language.UpdatePassword,
            Width = 450,
            Content = b => b.Div("kui-form-pwd", () =>
            {
                b.Component<PasswordForm>()
                 .Set(c => c.TipText, Config.System.TipChangePwd)
                 .Set(c => c.OnSave, async info =>
                 {
                     var result = await Admin.UpdatePasswordAsync(info);
                     UI.Result(result, model.CloseAsync);
                 })
                 .Build();
            }),
            Closable = false,
            Footer = null
        };
        UI.ShowDialog(model);
    }
}