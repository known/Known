namespace Known.Components;

/// <summary>
/// 管理后台模板页类。
/// </summary>
public class AdminLayout : LayoutComponentBase
{
    /// <summary>
    /// 呈现模板内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<KAdminLayout>()
               .Set(c => c.ChildContent, Body)
               .Build();
    }
}

/// <summary>
/// 管理后台模板组件类。
/// </summary>
public class KAdminLayout : KLayout
{
    private IAuthService Auth;

    /// <summary>
    /// 取得或设置注入的身份认证状态提供者实例。
    /// </summary>
    [Inject] protected IAuthStateProvider AuthProvider { get; set; }

    /// <summary>
    /// 取得后台管理主页数据对象。
    /// </summary>
    private AdminInfo Info { get; set; }

    /// <summary>
    /// 异步设置当前登录用户信息。
    /// </summary>
    /// <param name="user">用户信息。</param>
    /// <returns></returns>
    public override async Task SignInAsync(UserInfo user)
    {
        if (AuthProvider != null)
            await AuthProvider.SignInAsync(user);
    }

    /// <summary>
    /// 异步注销，用户安全退出系统。
    /// </summary>
    /// <returns></returns>
    public override async Task SignOutAsync()
    {
        var user = await AuthProvider.GetUserAsync();
        var result = await Auth.SignOutAsync();
        if (result.IsValid)
        {
            Context.SignOut();
            await SignInAsync(null);
            Navigation?.GoLoginPage();
            Config.OnExit?.Invoke();
        }
    }

    /// <summary>
    /// 异步初始化模板。
    /// 如果系统未安装，则跳转到安装页面；
    /// 如果系统未登录，则跳转到登录页面。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitAsync()
    {
        IsUrlAuth = true;
        IsLoaded = false;
        await base.OnInitAsync();
        Auth = await CreateServiceAsync<IAuthService>();
        if (AdminConfig.System == null)
        {
            var service = await CreateServiceAsync<ISystemService>();
            AdminConfig.System = await service.GetSystemAsync();
        }
        if (AdminConfig.System == null)
        {
            Navigation?.GoInstallPage();
        }
        else
        {
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                Context.CurrentUser = user;
                Info = await Auth.GetAdminAsync();
                Context.UserSetting = Info?.UserSetting ?? new();
                Context.UserTableSettings = Info?.UserTableSettings ?? [];
                if (!Context.IsMobileApp)
                    UserMenus = GetUserMenus(Info?.UserMenus);
                Cache.AttachCodes(Info?.Codes);
                Setting = Context.UserSetting;
                IsLoaded = true;
            }
            else
            {
                Navigation?.GoLoginPage();
            }
        }
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

    /// <summary>
    /// 异步获取第三方用户登录虚方法。
    /// </summary>
    /// <returns>用户信息。</returns>
    protected virtual Task<UserInfo> GetThirdUserAsync()
    {
        UserInfo user = null;
        return Task.FromResult(user);
    }

    private List<MenuInfo> GetUserMenus(List<MenuInfo> menus)
    {
        Context.UserMenus = menus;
        return menus.ToMenuItems();
    }
}