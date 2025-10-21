namespace Known.Pages;

/// <summary>
/// 登录页面组件类。
/// </summary>
[AllowAnonymous]
public class LoginPage : BasePage
{
    [Inject] private IAuthStateProvider AuthProvider { get; set; }

    /// <summary>
    /// 取得是否记住密码。
    /// </summary>
    protected virtual bool IsRememberPassword { get; }

    /// <summary>
    /// 取得或设置注册表单信息。
    /// </summary>
    [SupplyParameterFromForm] public RegisterFormInfo Register { get; set; } = new();

    /// <summary>
    /// 取得或设置登录表单信息。
    /// </summary>
    [SupplyParameterFromForm] public LoginFormInfo Model { get; set; } = new();

    /// <summary>
    /// 取得或设置登录成功后返回的URL。
    /// </summary>
    [SupplyParameterFromQuery] public string ReturnUrl { get; set; }

    /// <inheritdoc />
    protected override Task OnInitAsync()
    {
        Context.TabsService?.CloseAll();
        return base.OnInitAsync();
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            await SetCurrentUserAsync(null);
            var info = await JS.GetLoginInfoAsync<LoginInfo>();
            if (info != null)
            {
                Model.UserName = info.UserName;
                if (IsRememberPassword)
                    Model.Password = info.Password;
                Model.PhoneNo = info.PhoneNo;
                Model.Remember = info.Remember;
                Model.Station = info.Station;
                Model.TabKey = info.TabKey;
                await StateChangedAsync();
            }
        }
    }

    /// <summary>
    /// 注册提交前调用的验证。
    /// </summary>
    protected virtual Task<bool> OnRegisteringAsync() => Task.FromResult(true);

    /// <summary>
    /// 登录提交前调用的验证。
    /// </summary>
    protected virtual Task<bool> OnLoginingAsync() => Task.FromResult(true);

    /// <summary>
    /// 登录提交成功后调用。
    /// </summary>
    /// <param name="user">登录用户信息。</param>
    protected virtual Task OnLoginedAsync(UserInfo user)
    {
        Context.GoHomePage(ReturnUrl);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 注册按钮事件方法。
    /// </summary>
    /// <returns></returns>
    protected async Task OnUserRegister()
    {
        if (!await OnRegisteringAsync())
            return;

        Register.ClientId = Context.Local.ClientId;
        Register.IPAddress = Context.IPAddress;
        var result = await Admin.RegisterAsync(Register);
        await HandleResultAsync(result);
    }

    /// <summary>
    /// 登录按钮事件方法。
    /// </summary>
    /// <returns></returns>
    protected async Task OnUserLogin()
    {
        if (!await OnLoginingAsync())
            return;

        Model.ClientId = Context.Local.ClientId;
        Model.IPAddress = Context.IPAddress;
        var result = await Admin.SignInAsync(Model);
        await HandleResultAsync(result);
    }

    /// <summary>
    /// 调用身份认证提供者，异步设置当前登录用户信息。
    /// </summary>
    /// <param name="user">当前用户信息。</param>
    /// <returns></returns>
    protected virtual async Task SetCurrentUserAsync(UserInfo user)
    {
        var sessionId = await AuthProvider?.SignInAsync(user);
        await JSRuntime.InvokeVoidAsync("KNotify.addSession", sessionId);
    }

    private async Task HandleResultAsync(Result result)
    {
        if (!result.IsValid)
        {
            UI.Error(result.Message);
            return;
        }

        var user = result.DataAs<UserInfo>();
        if (user != null)
        {
            if (!IsStatic)
            {
                if (!Model.Remember)
                {
                    await JS.SetLoginInfoAsync(null);
                }
                else
                {
                    var info = new LoginInfo
                    {
                        UserName = Model.UserName,
                        PhoneNo = Model.PhoneNo,
                        Remember = Model.Remember,
                        Station = Model.Station,
                        TabKey = Model.TabKey
                    };
                    if (IsRememberPassword)
                        info.Password = Model.Password;
                    await JS.SetLoginInfoAsync(info);
                }
            }
            await SetCurrentUserAsync(user);
            await OnLoginedAsync(user);
        }
    }

    class LoginInfo
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PhoneNo { get; set; }
        public bool Remember { get; set; }
        public string Station { get; set; }
        public string TabKey { get; set; }
    }
}