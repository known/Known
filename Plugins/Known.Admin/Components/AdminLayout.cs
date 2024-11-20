namespace Known.Components;

/// <summary>
/// 管理后台模板组件类。
/// </summary>
public class AdminLayout : LayoutComponentBase
{
    /// <summary>
    /// 呈现模板内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<InnerLayout>()
               .Set(c => c.ChildContent, Body)
               .Build();
    }
}

class InnerLayout : KLayout
{
    private ISystemService System;
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
        IsLoaded = false;
        await base.OnInitAsync();
        System = await CreateServiceAsync<ISystemService>();
        Auth = await CreateServiceAsync<IAuthService>();
        if (Context.System == null)
            Context.System = await System.GetSystemAsync();
        if (Context.System == null)
        {
            Navigation?.NavigateTo("/install", true);
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
    /// 异步设置组件参数。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnParameterAsync()
    {
        await base.OnParameterAsync();
        if (!UIConfig.IgnoreRoutes.Contains(Context.Url) && !RouteData.PageType.IsAllowAnonymous())
        {
            if (Context.Current == null)
            {
                Navigation.GoErrorPage("403");
                return;
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