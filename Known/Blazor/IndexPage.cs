using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Known.Blazor;

public class IndexPage : BaseComponent
{
    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; }
    [Inject] private AuthenticationStateProvider AuthProvider { get; set; }

    protected bool IsLogin { get; private set; }
    public string Theme { get; private set; }
    public virtual string LogoUrl => Theme == "dark" ? "img/logo.png" : "img/logo1.png";

    protected override async Task OnInitAsync()
    {
        IsLoaded = false;
        if (Config.App.IsTheme)
            Theme = await JS.GetCurrentThemeAsync();
        Context.CurrentLanguage = await JS.GetCurrentLanguageAsync();
        Context.Install = await Platform.System.GetInstallAsync();
        Context.CurrentUser = await GetCurrentUserAsync();
        IsLogin = Context.CurrentUser != null;
        IsLoaded = true;
    }

    public void SetTheme(string theme)
    {
        if (!Config.App.IsTheme)
            return;

        Theme = theme;
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

    protected async Task OnLogin(UserInfo user)
    {
        await SetUserInfoAsync(user);

        var state = GetWeixinAuthState(user.Token);
        var uri = await Platform.Weixin.GetAuthorizeUrlAsync(state);
        if (IsLogin && !string.IsNullOrWhiteSpace(uri) && string.IsNullOrWhiteSpace(user.OpenId))
        {
            if (IsMobile)
                Navigation.NavigateTo(uri, true);
            else
                ShowWeixinQRCode(uri, user);
        }

        StateChanged();
    }

    protected async Task OnLogout()
    {
        Context.CurrentUser = null;
        IsLogin = false;
        await SetCurrentUserAsync(null);
        StateChanged();
    }

    protected virtual string GetWeixinAuthState(string token)
    {
        var url = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host;
        return $"{url}/weixin?token={token}&";
    }

    protected virtual void ShowWeixinQRCode(string uri, UserInfo user)
    {
        var isManualClose = false;
        var model = GetWeixinDialogModel(uri);
        model.OnClosed = () => isManualClose = true;
        UI.ShowDialog(model);
        Task.Run(async () =>
        {
            while (true)
            {
                if (isManualClose)
                {
                    Logger.Info("[WeixinQRCode] Scanning Manual Closed!");
                    break;
                }

                var weixin = await Platform.Weixin.CheckWeixinAsync(user);
                if (weixin != null)
                {
                    await SetUserInfoAsync(weixin);
                    await model.CloseAsync();
                    break;
                }
                Thread.Sleep(1000);
            }
        });
    }

    private DialogModel GetWeixinDialogModel(string uri)
    {
        var option = new { Text = uri, Width = 250, Height = 250 };
        return new DialogModel
        {
            Title = Language.GetString("WeixinQRCodeAuth"),
            Width = 300,
            Content = b => b.Component<KQRCode>().Set(c => c.Option, option).Build()
        };
    }

    private async Task SetUserInfoAsync(UserInfo user)
    {
        Context.CurrentUser = user;
        IsLogin = Context.CurrentUser != null;
        await SetCurrentUserAsync(user);
    }
}