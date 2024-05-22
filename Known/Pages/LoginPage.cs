using System.Security.Claims;

namespace Known.Pages;

public class LoginPage : BaseComponent
{
    [Inject] private AuthenticationStateProvider AuthProvider { get; set; }

    protected LoginFormInfo Model = new();

    //protected override async Task OnInitAsync()
    //{
    //    var state = GetWeixinAuthState(user.Token);
    //    var uri = await Platform.Weixin.GetAuthorizeUrlAsync(state);
    //    if (IsLogin && !string.IsNullOrWhiteSpace(uri) && string.IsNullOrWhiteSpace(user.OpenId))
    //    {
    //        if (IsMobile)
    //            NavigateWeixinAuth(uri, user);
    //        else
    //            ShowWeixinQRCode(uri, user);
    //    }
    //}

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            var info = await JS.GetLoginInfoAsync<LoginInfo>();
            if (info != null)
            {
                Model.UserName = info.UserName;
                Model.PhoneNo = info.PhoneNo;
                Model.Remember = info.Remember;
                Model.Station = info.Station;
                Model.TabKey = info.TabKey;
            }
        }
    }

    protected void SetTheme(string theme)
    {
        if (!Config.App.IsTheme)
            return;

        Context.Theme = theme;
        StateChanged();
    }

    protected virtual void OnLogining() { }
    protected virtual void OnLogined()
    {
        if (Context.IsMobile)
            Navigation.NavigateTo("/app");
        else
            Navigation.NavigateTo("/");
    }

    protected async Task OnUserLogin()
    {
        if (!Model.Remember)
        {
            await JS.SetLoginInfoAsync(null);
        }
        else
        {
            await JS.SetLoginInfoAsync(new LoginInfo
            {
                UserName = Model.UserName,
                PhoneNo = Model.PhoneNo,
                Remember = Model.Remember,
                Station = Model.Station,
                TabKey = Model.TabKey
            });
        }

        OnLogining();
        Model.IPAddress = HttpContext?.Connection?.RemoteIpAddress?.ToString();
        var result = await Platform.Auth.SignInAsync(Model);
        if (!result.IsValid)
        {
            UI.Error(result.Message);
        }
        else
        {
            var user = result.DataAs<UserInfo>();
            //await SetCurrentUserAsync(user);
            //var identity = new GenericIdentity(user.UserName, "Forms");
            //var principal = new GenericPrincipal(identity, user.Role.Split(','));
            var identity = new ClaimsIdentity("Forms");
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            var principal = new ClaimsPrincipal(identity);
            HttpContext.User = principal;
            OnLogined();
        }
    }

    protected virtual async Task SetCurrentUserAsync(UserInfo user)
    {
        if (AuthProvider is IAuthStateProvider provider)
        {
            await provider.UpdateUserAsync(user);
        }
    }

    protected virtual string GetWeixinAuthState(string token)
    {
        var url = HttpContext.GetHostUrl();
        return $"{url}/?token={token}&";
    }

    protected virtual DialogModel GetWeixinDialogModel(string uri)
    {
        var option = new { Text = uri, Width = 250, Height = 250 };
        return new DialogModel
        {
            Title = Language.GetString("WeixinQRCodeAuth"),
            Width = 300,
            Content = b => b.Component<KQRCode>().Set(c => c.Option, option).Build()
        };
    }

    private void NavigateWeixinAuth(string uri, UserInfo user)
    {
        Task.Run(async () =>
        {
            while (true)
            {
                var weixin = await Platform.Weixin.CheckWeixinAsync(user);
                if (weixin != null)
                {
                    //await SetUserInfoAsync(weixin);
                    await UI.Toast(Language.Success(Language.Authorize));
                    break;
                }
                Thread.Sleep(1000);
            }
        });
        Navigation.NavigateTo(uri);
    }

    private void ShowWeixinQRCode(string uri, UserInfo user)
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
                    //await SetUserInfoAsync(weixin);
                    await model.CloseAsync();
                    await UI.Toast(Language.Success(Language.Authorize));
                    break;
                }
                Thread.Sleep(1000);
            }
        });
    }

    class LoginInfo
    {
        public string UserName { get; set; }
        public string PhoneNo { get; set; }
        public bool Remember { get; set; }
        public string Station { get; set; }
        public string TabKey { get; set; }
    }
}