namespace Known.Pages;

/// <summary>
/// 登录页面组件类。
/// </summary>
public class LoginPage : BaseComponent
{
    [Inject] private IAuthStateProvider AuthProvider { get; set; }

    /// <summary>
    /// 登录表单信息。
    /// </summary>
    [SupplyParameterFromForm] public LoginFormInfo Model { get; set; } = new();

    /// <summary>
    /// 取得或设置登录成功后返回的URL。
    /// </summary>
    [SupplyParameterFromQuery] public string ReturnUrl { get; set; }

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
                await StateChangedAsync();
            }
        }
    }

    /// <summary>
    /// 登录提交前调用的验证虚方法。
    /// </summary>
    protected virtual Task<bool> OnLoginingAsync() => Task.FromResult(true);

    /// <summary>
    /// 登录提交成功后调用的虚方法。
    /// </summary>
    /// <param name="user">登录用户信息。</param>
    protected virtual async Task OnLoginedAsync(UserInfo user)
    {
        if (IsStatic)
            return;

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

        var url = Context.IsMobileApp ? "/app" : (ReturnUrl ?? "/");
        Navigation.NavigateTo(url, true);
    }

    /// <summary>
    /// 登录按钮事件方法。
    /// </summary>
    /// <returns></returns>
    protected async Task OnUserLogin()
    {
        if (!await OnLoginingAsync())
            return;

        Model.IPAddress = Context.IPAddress;
        var result = await Platform.SignInAsync(Model);
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
                await OnLoginedAsync(user);
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
        await AuthProvider?.SignInAsync(user);
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