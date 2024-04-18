namespace Known.Blazor;

public class LoginPage : BaseComponent
{
    protected LoginFormInfo Model = new();

    [Parameter] public Func<UserInfo, Task> OnLogin { get; set; }

    protected override async Task OnInitAsync()
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

    protected virtual void OnSigning() { }

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

        OnSigning();
        Model.IPAddress = HttpContext?.Connection?.RemoteIpAddress?.ToString();
        var result = await Platform.Auth.SignInAsync(Model);
        if (!result.IsValid)
        {
            UI.Error(result.Message);
        }
        else
        {
            var user = result.DataAs<UserInfo>();
            await OnLogin?.Invoke(user);
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
                    UI.Toast(Language.Success(Language.Authorize));
                    break;
                }
                Thread.Sleep(1000);
            }
        });
        Navigation.NavigateTo(uri, true);
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
                    UI.Toast(Language.Success(Language.Authorize));
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