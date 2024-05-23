﻿namespace Known.Blazor;

public class PageLayout : BaseLayout
{
    protected bool IsLoaded { get; private set; }
    protected AdminInfo Info { get; private set; }
    protected List<MenuItem> UserMenus { get; private set; }
    protected bool IsLogin { get; private set; }

    //[CascadingParameter] private Task<AuthenticationState> AuthState { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            IsLoaded = false;
            Context.CurrentUser = await GetCurrentUserAsync();
            IsLogin = Context.CurrentUser != null;
            if (IsLogin)
            {
                if (!Context.IsMobile)
                {
                    Info = await Platform.Auth.GetAdminAsync();
                    UserMenus = GetUserMenus(Info?.UserMenus);
                    Context.UserSetting = Info?.UserSetting ?? new();
                }
                IsLoaded = true;
            }
        }
        catch (Exception ex)
        {
            await OnError(ex);
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
            await Context.SetCurrentMenuAsync(Platform, pageId);
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

        //if (AuthState == null)
        //    return null;

        //var state = await AuthState;
        //if (state != null && state.User != null && state.User.Identity != null && state.User.Identity.IsAuthenticated)
        //{
        //    if (AuthProvider is IAuthStateProvider provider)
        //        return await provider.GetUserAsync();
        //}

        return await GetThirdUserAsync();
    }

    private List<MenuItem> GetUserMenus(List<MenuInfo> menus)
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