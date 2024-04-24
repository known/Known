using Microsoft.Net.Http.Headers;

namespace Known.Blazor;

public class PageLayout : BaseLayout
{
    protected bool IsLoaded { get; private set; }
    protected AdminInfo Info { get; private set; }
    protected List<MenuItem> UserMenus { get; private set; }
    protected bool IsLogin { get; private set; }

    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; }
    [Inject] private IHttpContextAccessor HttpAccessor { get; set; }
    [Inject] public JSService JS { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            IsLoaded = false;
            var httpContext = HttpAccessor?.HttpContext;
            Context.IsMobile = CheckMobile(httpContext?.Request);
            if (Config.App.IsTheme)
                Context.Theme = await JS.GetCurrentThemeAsync();
            Context.Host = httpContext?.GetHostUrl();
            Context.CurrentLanguage = await JS.GetCurrentLanguageAsync();
            Context.Install = await Platform.System.GetInstallAsync();
            if (!Context.Install.IsInstalled)
            {
                NavigateTo("/install");
            }
            else
            {
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
                    await ShowNoticeAsync(Context.CurrentUser);
                }
                else
                {
                    NavigateTo("/login");
                }
            }
        }
        catch (Exception ex)
        {
            await OnError(ex);
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

    protected virtual Task ShowNoticeAsync(UserInfo user) => Task.CompletedTask;

    protected virtual Task<UserInfo> GetThirdUserAsync()
    {
        UserInfo user = null;
        return Task.FromResult(user);
    }

    protected virtual async Task<UserInfo> GetCurrentUserAsync()
    {
        if (AuthState == null)
            return null;

        var state = await AuthState;
        if (state != null && state.User != null && state.User.Identity != null && state.User.Identity.IsAuthenticated)
        {
            if (AuthProvider is IAuthStateProvider provider)
                return await provider.GetUserAsync();
        }

        return await GetThirdUserAsync();
    }

    private List<MenuItem> GetUserMenus(List<MenuInfo> menus)
    {
        Context.UserMenus = menus;
        return menus.ToMenuItems();
    }

    private static bool CheckMobile(HttpRequest request)
    {
        if (request == null)
            return false;

        var agent = request.Headers[HeaderNames.UserAgent].ToString();
        return Utils.CheckMobile(agent);
    }
}