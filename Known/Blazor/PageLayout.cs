namespace Known.Blazor;

public class PageLayout : BaseLayout
{
    protected bool IsLoaded { get; private set; }
    protected AdminInfo Info { get; private set; }
    protected List<MenuInfo> UserMenus { get; private set; }
    protected bool IsLogin { get; private set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        IsLoaded = false;
        if (!Config.IsInstalled)
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
                if (!Context.IsMobile)
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

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            //非Server模式，JS不能在初始化中调用
            if (Config.App.IsTheme)
                Context.Theme = await JS.GetCurrentThemeAsync();
            Context.CurrentLanguage = await JS.GetCurrentLanguageAsync();
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        try
        {
            var pageId = "";
            Context.Url = Navigation.GetPageUrl();
            if (Context.Url.StartsWith("/page/"))
                pageId = Context.Url.Split("/")[2];
            //Logger.Info($"Layout={Context.Url}");
            await base.OnParametersSetAsync();
            var service = await Factory.CreateAsync<ISystemService>(Context);
            await Context.SetCurrentMenuAsync(service, pageId);
        }
        catch (Exception ex)
        {
            await OnError(ex);
        }
    }

    protected virtual Task<UserInfo> GetThirdUserAsync()
    {
        UserInfo user = null;
        return Task.FromResult(user);
    }

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

public class EmptyLayout : BaseLayout
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<KLayout>()
               .Set(c => c.Layout, this)
               .Set(c => c.ChildContent, Body)
               .Build();
    }
}