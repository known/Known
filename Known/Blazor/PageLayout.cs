namespace Known.Blazor;

/// <summary>
/// 页面布局模板组件类。
/// </summary>
public class PageLayout : BaseLayout
{
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
            NavigateTo("/install");
        }
        else
        {
            var user = await GetCurrentUserAsync();
            IsLogin = user != null;
            if (IsLogin)
            {
                Context.CurrentUser = user;
                Info = await AuthService.GetAdminAsync();
                Context.UserSetting = Info?.UserSetting ?? new();
                Context.UserTableSettings = Info?.UserTableSettings ?? [];
                if (!Context.IsMobileApp)
                    UserMenus = GetUserMenus(Info?.UserMenus);
                Cache.AttachCodes(Info?.Codes);
                IsLoaded = true;
            }
            else
            {
                NavigateTo("/login");
            }
        }
    }

    /// <summary>
    /// 异步设置模板页参数。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnParametersSetAsync()
    {
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
            if (Context.Current != null && !Config.IsClient)
            {
                await SystemService.AddLogAsync(new SysLog
                {
                    Target = Context.Current.Name,
                    Content = Context.Url,
                    Type = LogType.Page.ToString()
                });
            }
        }
        catch (Exception ex)
        {
            await OnError(ex);
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
}

/// <summary>
/// 空模板组件类。
/// </summary>
public class EmptyLayout : BaseLayout
{
    /// <summary>
    /// 呈现空模板组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<KLayout>()
               .Set(c => c.Layout, this)
               .Set(c => c.ChildContent, Body)
               .Build();
    }
}