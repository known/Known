namespace Known.Pages;

public class LoginPage : BaseComponent
{
    private IAuthService authService;
    //private IWeixinService weixinService;
    [Inject] private IAuthStateProvider AuthProvider { get; set; }

    protected LoginFormInfo Model = new();

    [SupplyParameterFromQuery] public string ReturnUrl { get; set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        authService = await CreateServiceAsync<IAuthService>();
        //weixinService = await CreateServiceAsync<IWeixinService>();
        //var state = GetWeixinAuthState(user.Token);
        //var uri = await Platform.Weixin.GetAuthorizeUrlAsync(state);
        //if (IsLogin && !string.IsNullOrWhiteSpace(uri) && string.IsNullOrWhiteSpace(user.OpenId))
        //{
        //    if (IsMobile)
        //        NavigateWeixinAuth(uri, user);
        //    else
        //        ShowWeixinQRCode(uri, user);
        //}
    }

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

    protected virtual void OnLogining() { }
    protected virtual void OnLogined(UserInfo user)
    {
        if (Context.IsMobile)
            Navigation.NavigateTo("/app");
        else
            Navigation.NavigateTo(ReturnUrl ?? "/");
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
        Model.IPAddress = Context.IPAddress;
        var result = await authService.SignInAsync(Model);
        if (!result.IsValid)
        {
            UI.Error(result.Message);
        }
        else
        {
            var user = result.DataAs<UserInfo>();
            await SetCurrentUserAsync(user);
            OnLogined(user);
        }
    }

    protected virtual async Task SetCurrentUserAsync(UserInfo user)
    {
        await AuthProvider?.SetUserAsync(user);
    }

    //protected virtual string GetWeixinAuthState(string token)
    //{
    //    var url = Config.HostUrl;
    //    return $"{url}/?token={token}&";
    //}

    //protected virtual DialogModel GetWeixinDialogModel(string uri)
    //{
    //    var option = new { Text = uri, Width = 250, Height = 250 };
    //    return new DialogModel
    //    {
    //        Title = Language.GetString("WeixinQRCodeAuth"),
    //        Width = 300,
    //        Content = b => b.Component<KQRCode>().Set(c => c.Option, option).Build()
    //    };
    //}

    //private void NavigateWeixinAuth(string uri, UserInfo user)
    //{
    //    Task.Run(async () =>
    //    {
    //        while (true)
    //        {
    //            var weixin = await weixinService.CheckWeixinAsync(user);
    //            if (weixin != null)
    //            {
    //                //await SetUserInfoAsync(weixin);
    //                await UI.Toast(Language.Success(Language.Authorize));
    //                break;
    //            }
    //            Thread.Sleep(1000);
    //        }
    //    });
    //    Navigation.NavigateTo(uri);
    //}

    //private void ShowWeixinQRCode(string uri, UserInfo user)
    //{
    //    var isManualClose = false;
    //    var model = GetWeixinDialogModel(uri);
    //    model.OnClosed = () => isManualClose = true;
    //    UI.ShowDialog(model);
    //    Task.Run(async () =>
    //    {
    //        while (true)
    //        {
    //            if (isManualClose)
    //            {
    //                Logger.Info("[WeixinQRCode] Scanning Manual Closed!");
    //                break;
    //            }

    //            var weixin = await weixinService.CheckWeixinAsync(user);
    //            if (weixin != null)
    //            {
    //                //await SetUserInfoAsync(weixin);
    //                await model.CloseAsync();
    //                await UI.Toast(Language.Success(Language.Authorize));
    //                break;
    //            }
    //            Thread.Sleep(1000);
    //        }
    //    });
    //}

    class LoginInfo
    {
        public string UserName { get; set; }
        public string PhoneNo { get; set; }
        public bool Remember { get; set; }
        public string Station { get; set; }
        public string TabKey { get; set; }
    }
}