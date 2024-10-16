namespace Known.Pages;

/// <summary>
/// 登录页面组件类。
/// </summary>
public class LoginPage : BaseComponent
{
    private IAuthService Service;
    //private IWeixinService weixinService;
    [Inject] private IAuthStateProvider AuthProvider { get; set; }

    /// <summary>
    /// 登录表单信息。
    /// </summary>
    protected LoginFormInfo Model = new();

    /// <summary>
    /// 取得或设置登录成功后返回的URL。
    /// </summary>
    [SupplyParameterFromQuery] public string ReturnUrl { get; set; }

    /// <summary>
    /// 异步初始化登录组件。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Service = await CreateServiceAsync<IAuthService>();
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

    /// <summary>
    /// 登录组件呈现后，调用JS获取本地记忆的用户名。
    /// </summary>
    /// <param name="firstRender">是否首次呈现。</param>
    /// <returns></returns>
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

    /// <summary>
    /// 登录提交前调用的验证虚方法。
    /// </summary>
    protected virtual bool OnLogining() => true;

    /// <summary>
    /// 登录提交成功后调用的虚方法。
    /// </summary>
    /// <param name="user">登录用户信息。</param>
    protected virtual void OnLogined(UserInfo user)
    {
        var url = Context.IsMobileApp ? "/app" : (ReturnUrl ?? "/");
        Navigation.NavigateTo(url);
    }

    /// <summary>
    /// 登录按钮事件方法。
    /// </summary>
    /// <returns></returns>
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

        if (!OnLogining())
            return;

        Model.IPAddress = Context.IPAddress;
        var result = await Service.SignInAsync(Model);
        if (!result.IsValid)
        {
            UI.Error(result.Message);
        }
        else
        {
            var user = result.DataAs<UserInfo>();
            if (user != null)
            {
                await SetCurrentUserAsync(user);
                OnLogined(user);
            }
        }
    }

    /// <summary>
    /// 调用身份认证提供者，异步设置当前登录用户信息
    /// </summary>
    /// <param name="user">当前用户信息。</param>
    /// <returns></returns>
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