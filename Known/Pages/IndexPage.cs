namespace Known.Pages;

public class IndexPage : BaseComponent
{
    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; }
    [Inject] private AuthenticationStateProvider AuthProvider { get; set; }

    protected bool IsLogin { get; private set; }
    public virtual string LogoUrl => Context.LogoUrl;

    protected override async Task OnInitAsync()
    {
        IsLoaded = false;
        if (Config.App.IsTheme)
            Context.Theme = await JS.GetCurrentThemeAsync();
        Context.Host = HttpContext.GetHostUrl();
        Context.CurrentLanguage = await JS.GetCurrentLanguageAsync();
        Context.Install = await Platform.System.GetInstallAsync();
        if (!Context.Install.IsInstalled)
        {
            Navigation.NavigateTo("/install", true);
        }
        else
        {
            Context.CurrentUser = await GetCurrentUserAsync();
            IsLogin = Context.CurrentUser != null;
            if (IsLogin)
            {
                await ShowNoticeAsync(Context.CurrentUser);
                IsLoaded = true;
            }
            else
            {
                Navigation.NavigateTo("/login", true);
            }
        }
    }

    public void SetTheme(string theme)
    {
        if (!Config.App.IsTheme)
            return;

        Context.Theme = theme;
        StateChanged();
    }

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

    protected virtual async Task SetCurrentUserAsync(UserInfo user)
    {
        if (AuthProvider is IAuthStateProvider provider)
        {
            await provider.UpdateUserAsync(user);
        }
    }

    protected void OnInstall(InstallInfo install)
    {
        Context.Install = install;
        StateChanged();
    }

    //protected async Task OnLogin(UserInfo user)
    //{
    //    await SetUserInfoAsync(user);
    //    await ShowNoticeAsync(user);
    //    StateChanged();
    //}

    protected async Task OnLogout()
    {
        Context.CurrentUser = null;
        IsLogin = false;
        await SetCurrentUserAsync(null);
        //StateChanged();
        Navigation.NavigateTo("/login", true);
    }

    protected virtual Task ShowNoticeAsync(UserInfo user) => Task.CompletedTask;

    //private async Task SetUserInfoAsync(UserInfo user)
    //{
    //    Context.CurrentUser = user;
    //    IsLogin = Context.CurrentUser != null;
    //    await SetCurrentUserAsync(user);
    //}
}