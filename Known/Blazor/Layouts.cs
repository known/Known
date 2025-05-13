namespace Known.Blazor;

/// <summary>
/// 模板组件基类。
/// </summary>
public class LayoutBase : LayoutComponentBase
{
    internal bool IsLoaded { get; set; } = true;
    internal IAdminService Admin { get; set; }

    [CascadingParameter] internal UIContext Context { get; set; }
    [Inject] internal IServiceScopeFactory Factory { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Admin = await Factory.CreateAsync<IAdminService>(Context);

        IsLoaded = false;
        var info = await Admin.GetInitialAsync();
        if (info != null)
        {
            Language.Settings = info.LanguageSettings;
            Language.Datas = info.Languages;
            Config.OnInitial?.Invoke(info);
            UIConfig.Load(info);
        }
        IsLoaded = true;
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
        builder.Div("kui-wrapper", () => builder.Fragment(Body));
    }
}

/// <summary>
/// 管理后台模板页类。
/// </summary>
public class AdminLayout : LayoutBase
{
    [Inject] private IAuthStateProvider AuthProvider { get; set; }
    [Inject] private NavigationManager Navigation { get; set; }
    [Inject] private JSService JS { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        IsLoaded = false;
        if (!Config.IsInstalled)
        {
            var isInstall = await Admin.GetIsInstallAsync(); //检查是否需要安装
            Config.IsInstalled = !isInstall;
        }
        if (!Config.IsInstalled)
        {
            Navigation?.GoInstallPage();
            return;
        }

        Context.CurrentUser = await GetCurrentUserAsync();
        if (Context.CurrentUser == null)
        {
            Navigation?.GoLoginPage();
            return;
        }
        IsLoaded = true;
    }

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

    private async Task<UserInfo> GetCurrentUserAsync()
    {
        if (AuthProvider == null)
            return null;

        return await AuthProvider.GetUserAsync();
    }
}