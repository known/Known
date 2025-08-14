namespace Known.Blazor;

/// <summary>
/// 模板组件基类。
/// </summary>
public class LayoutBase : LayoutComponentBase
{
    internal bool IsInstall { get; private set; }
    internal bool IsLoaded { get; set; }

    [Inject] internal IServiceScopeFactory Factory { get; set; }
    [Inject] internal JSService JS { get; set; }

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

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Admin = await Factory.CreateAsync<IAdminService>(Context);
        Context.UI = UI;
        Context.Navigation = Navigation;
        IsInstall = false;
        IsLoaded = false;
        var info = await Admin.GetInitialAsync();
        if (info != null)
        {
            Language.Settings = info.LanguageSettings;
            Language.Datas = info.Languages;
            Config.OnInitial?.Invoke(info);
            UIConfig.Load(info);
        }

        if (!Config.IsInstalled)
        {
            var isInstall = await Admin.GetIsInstallAsync(); //检查是否需要安装
            Config.IsInstalled = !isInstall;
        }
        if (!Config.IsInstalled)
        {
            IsInstall = true;
            Navigation?.GoInstallPage();
            return;
        }

        IsInstall = true;
        IsLoaded = await OnInitAsync();
    }

    /// <summary>
    /// 异步初始化模板组件。
    /// </summary>
    /// <returns></returns>
    protected virtual Task<bool> OnInitAsync() => Task.FromResult(true);

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            if (Config.App.IsLanguage)
            {
                var language = await JS.GetCurrentLanguageAsync();
                if (string.IsNullOrWhiteSpace(language))
                    language = Context.UserSetting.Language;
                Context.CurrentLanguage = language;
            }

            var setting = Context.UserSetting;
            if (string.IsNullOrWhiteSpace(setting.Size))
                setting.Size = Config.App.DefaultSize;
            await JS.SetUserSettingAsync(setting);
        }
    }
}

/// <summary>
/// 空模板组件类。
/// </summary>
public class EmptyLayout : LayoutBase
{
    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!IsInstall)
            return;

        builder.Div("kui-wrapper", () => builder.Fragment(Body));
    }
}

/// <summary>
/// 身份验证模板组件类。
/// </summary>
public class AuthLayout : LayoutBase
{
    private bool isLayout;
    [Inject] private IAuthStateProvider AuthProvider { get; set; }

    /// <inheritdoc />
    protected override async Task<bool> OnInitAsync()
    {
        await base.OnInitAsync();
        Context.CurrentUser = await GetCurrentUserAsync();
        if (Context.CurrentUser == null)
        {
            Navigation?.GoLoginPage();
            return false;
        }
        if (UIConfig.OnInitLayout != null)
        {
            var isInit = UIConfig.OnInitLayout.Invoke(Context);
            isLayout = true;
            return isInit;
        }
        return true;
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (IsLoaded || isLayout)
            builder.Div("kui-wrapper", () => builder.Fragment(Body));
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
    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            if (Context.CurrentUser == null)
                await JS.InitFilesAsync();
        }
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!IsLoaded)
            return;

        builder.Div("kui-wrapper", () =>
        {
            if (Context.IsMobileApp)
            {
                builder.Component<AppLayout>().Set(c => c.ChildContent, BuildBody).Build();
            }
            else
            {
                if (UIConfig.AdminBody != null)
                    UIConfig.AdminBody.Invoke(builder, BuildBody);
                else
                    builder.Component<MainLayout>().Set(c => c.ChildContent, BuildBody).Build();
            }
        });
    }

    private void BuildBody(RenderTreeBuilder builder)
    {
        builder.Component<AuthPanel>().Set(c => c.ChildContent, Body).Build();
    }
}