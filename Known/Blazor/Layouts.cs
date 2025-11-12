namespace Known.Blazor;

/// <summary>
/// 模板组件基类。
/// </summary>
public class LayoutBase : LayoutComponentBase
{
    [Inject] internal IServiceScopeFactory Factory { get; set; }

    /// <summary>
    /// 取得或设置JS运行时实例。
    /// </summary>
    [Inject] public IJSRuntime JSRuntime { get; set; }

    /// <summary>
    /// 取得或设置注入的JS服务实例。
    /// </summary>
    [Inject] public JSService JS { get; set; }

    /// <summary>
    /// 取得或设置注入的UI服务实例。
    /// </summary>
    [Inject] public UIService UI { get; set; }

    /// <summary>
    /// 取得或设置注入的导航管理者实例。
    /// </summary>
    [Inject] public NavigationManager Navigation { get; set; }

    /// <summary>
    /// 取得或设置UI上下文对象级联值实例。
    /// </summary>
    [CascadingParameter] public UIContext Context { get; set; }

    /// <summary>
    /// 取得框架管理后台数据服务接口实例。
    /// </summary>
    public IAdminService Admin { get; private set; }

    /// <summary>
    /// 模板外套CSS样式类。
    /// </summary>
    protected string WrapperClass => CssBuilder.Default("kui-wrapper")
                                               .AddClass("kui-app", Context.IsMobileApp)
                                               .BuildClass();

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Admin = await Factory.CreateAsync<IAdminService>(Context);
        Context.UI = UI;
        Context.Navigation = Navigation;
        if (Context.Local == null)
        {
            Context.Local = await JS.GetLocalInfoAsync();
            Context.IsReload = true;
        }

        if (Context.IsInitial)
            return;

        Context.IsInitial = true;
        var info = await Admin.GetInitialAsync();
        if (info != null)
        {
            Language.Settings = info.LanguageSettings;
            Language.Datas = info.Languages;
            Config.HostUrl = info.HostUrl;
            Config.IsInstalled = info.IsInstalled;
            Config.System = info.System;
            Config.OnInitial?.Invoke(info);
            UIConfig.Load(info);
        }

        if (!Config.IsInstalled)
        {
            Navigation?.GoInstallPage();
            return;
        }

        if (info.ClientHomes?.TryGetValue(Context.Local.ClientId, out string homeUrl) == true)
        {
            Navigation?.NavigateTo(homeUrl);
            return;
        }

        await OnInitAsync();
    }

    /// <summary>
    /// 异步初始化模板组件。
    /// </summary>
    /// <returns></returns>
    protected virtual Task OnInitAsync() => Task.CompletedTask;

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender || Context.IsReload)
        {
            Context.IsReload = false;
            var setting = Context.UserSetting;
            if (setting != null && string.IsNullOrWhiteSpace(setting.Size))
                setting.Size = Config.App.DefaultSize;
            await JS.SetLocalInfoAsync(Context.Local, setting);
            if (Config.App.IsLanguage)
            {
                var language = Context.Local?.Language;
                if (string.IsNullOrWhiteSpace(language))
                    language = Context.UserSetting.Language;
                Context.CurrentLanguage = language;
            }
        }
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        BuildRender(builder);
    }

    /// <summary>
    /// 呈现组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected virtual void BuildRender(RenderTreeBuilder builder) { }

    internal static void BuildBackgroundBlobs(RenderTreeBuilder builder)
    {
        //builder.Div("background-blobs", () =>
        //{
        //    builder.Div("blob blob-1", "");
        //    builder.Div("blob blob-2", "");
        //});
    }
}

/// <summary>
/// 空模板组件类。
/// </summary>
public class EmptyLayout : LayoutBase
{
    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (Context.IsInitial)
        {
            BuildBackgroundBlobs(builder);
            builder.Div(WrapperClass, () => builder.Fragment(Body));
        }
    }
}

/// <summary>
/// 身份验证模板组件类。
/// </summary>
public class AuthLayout : LayoutBase
{
    [Inject] internal IAuthStateProvider AuthProvider { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Context.CurrentUser = await GetCurrentUserAsync();
        if (Context.CurrentUser == null)
        {
            Navigation?.GoLoginPage();
            return;
        }
        UIConfig.OnInitLayout?.Invoke(Context);
    }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (Context.IsInitial && Context.CurrentUser != null)
        {
            BuildBackgroundBlobs(builder);
            builder.Div(WrapperClass, () => builder.BuildBody(Context, BuildContent));
        }
    }

    internal virtual void BuildContent(RenderTreeBuilder builder)
    {
        builder.Fragment(Body);
    }

    private async Task<UserInfo> GetCurrentUserAsync()
    {
        if (AuthProvider == null)
            return null;

        return await AuthProvider.GetUserAsync();
    }
}

/// <summary>
/// 管理后台模板页类。
/// </summary>
public class AdminLayout : AuthLayout
{
    private DotNetObjectReference<AdminLayout> invoker;

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (!Context.IsNotifyHub)
        {
            Context.IsNotifyHub = true;
            if (Config.IsNotifyHub)
            {
                invoker = DotNetObjectReference.Create(this);
                await JSRuntime.InvokeJsAsync("KNotify.init", invoker, new
                {
                    NotifyUrl = Constants.NotifyHubUrl,
                    Constants.ForceLogout,
                    ShowForceLogout = nameof(ShowForceLogout),
                    Constants.NotifyLayout,
                    ShowNotify = nameof(ShowNotify)
                });
            }
        }
    }

    internal override void BuildContent(RenderTreeBuilder builder)
    {
        if (Context.IsMobileApp)
        {
            if (UIConfig.AppBody != null)
                UIConfig.AppBody.Invoke(builder, BuildBody);
            else
                builder.Component<AppLayout>().Set(c => c.ChildContent, BuildBody).Build();
        }
        else
        {
            if (UIConfig.AdminBody != null)
                UIConfig.AdminBody.Invoke(builder, BuildBody);
            else
                builder.Component<MainLayout>().Set(c => c.ChildContent, BuildBody).Build();
        }
    }

    /// <summary>
    /// 显示强制退出登录对话框。
    /// </summary>
    /// <param name="message">退出提示信息。</param>
    [JSInvokable]
    public void ShowForceLogout(string message)
    {
        UI.Alert(message, async () =>
        {
            await AuthProvider.SignOutAsync();
            Navigation?.GoLoginPage();
        });
    }

    /// <summary>
    /// 显示通知。
    /// </summary>
    /// <param name="message">通知信息。</param>
    [JSInvokable]
    public void ShowNotify(string message)
    {
        var info = Utils.FromJson<NotifyInfo>(message);
        if (info == null)
            return;

        UI.NoticeAsync(info.Title, info.Message, info.Type);
    }

    private void BuildBody(RenderTreeBuilder builder)
    {
        builder.Component<AuthPanel>().Set(c => c.ChildContent, Body).Build();
    }
}