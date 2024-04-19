using Microsoft.Net.Http.Headers;

namespace Known.Blazor;

public class AppLayout : LayoutComponentBase
{
    protected bool IsMobile { get; private set; }
    protected bool IsLoaded { get; set; }
    protected AdminInfo Info { get; private set; }
    protected List<MenuItem> UserMenus { get; private set; }
    protected bool IsLogin { get; private set; }

    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; }
    [Inject] private AuthenticationStateProvider AuthProvider { get; set; }
    [Inject] private IHttpContextAccessor HttpAccessor { get; set; }
    [Inject] public JSService JS { get; set; }
    [Inject] public NavigationManager Navigation { get; set; }
    [CascadingParameter] public Context Context { get; set; }
    public HttpContext HttpContext => HttpAccessor.HttpContext;
    public Language Language => Context?.Language;
    public IUIService UI => Context?.UI;
    public UserInfo CurrentUser => Context?.CurrentUser;
    public MenuItem CurrentMenu { get; private set; }

    private PlatformService platform;
    public PlatformService Platform
    {
        get
        {
            platform ??= new PlatformService(Context);
            return platform;
        }
    }
    protected override async Task OnInitializedAsync()
    {
        try
        {
            IsMobile = CheckMobile(HttpAccessor?.HttpContext?.Request);
            IsLoaded = false;
            if (Config.App.IsTheme)
                Context.Theme = await JS.GetCurrentThemeAsync();
            Context.Host = HttpContext.GetHostUrl();
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
                    if (IsMobile)
                    {
                        NavigateTo("/app");
                    }
                    else
                    {
                        CurrentMenu = Config.GetHomeMenu();
                        Info = await Platform.Auth.GetAdminAsync();
                        UserMenus = GetUserMenus(Info?.UserMenus);
                        Context.UserSetting = Info?.UserSetting ?? new();
                        await ShowNoticeAsync(Context.CurrentUser);
                        IsLoaded = true;
                    }
                }
                else
                {
                    NavigateTo("/login");
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
            await OnError(ex);
        }
    }

    public virtual Task ShowSpinAsync(string text = null) => Task.CompletedTask;
    public virtual void HideSpin() { }
    public virtual void StateChanged() => InvokeAsync(StateHasChanged);
    protected virtual Task ShowNoticeAsync(UserInfo user) => Task.CompletedTask;
    protected virtual Task OnError(Exception ex) => Task.CompletedTask;

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

    public void NavigateTo(string url) => Navigation.NavigateTo(url);

    public void NavigateTo(MenuItem item)
    {
        if (item == null)
            return;

        CurrentMenu = item;
        Context.Current = item;

        var url = item.Url;
        if (string.IsNullOrWhiteSpace(url) || item.Target == ModuleType.IFrame.ToString())
            url = $"/page?pid={item.Id}";
        NavigateTo(url);
    }

    public void Back()
    {
        if (CurrentMenu.Previous == null)
            return;

        NavigateTo(CurrentMenu.Previous);
    }

    public async Task SignOutAsync()
    {
        var user = CurrentUser;
        var result = await Platform.SignOutAsync(user?.Token);
        if (result.IsValid)
        {
            Context.CurrentUser = null;
            await SetCurrentUserAsync(null);
            NavigateTo("/login");
            Config.OnExit?.Invoke();
        }
    }

    private async Task SetCurrentUserAsync(UserInfo user)
    {
        if (AuthProvider is IAuthStateProvider provider)
        {
            await provider.UpdateUserAsync(user);
        }
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