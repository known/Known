namespace Known.Blazor;

/// <summary>
/// 空模板组件类。
/// </summary>
public class EmptyLayout : LayoutComponentBase
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
public class AdminLayout : LayoutComponentBase
{
    [CascadingParameter] private UIContext Context { get; set; }
    [Inject] private IAuthStateProvider AuthProvider { get; set; }
    [Inject] private IServiceScopeFactory Factory { get; set; }
    [Inject] private NavigationManager Navigation { get; set; }
    [Inject] private JSService JS { get; set; }
    private IAdminService Admin { get; set; }
    private bool IsLoaded { get; set; } = true;

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Admin = await Factory.CreateAsync<IAdminService>(Context);

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

        var user = await GetCurrentUserAsync();
        if (user == null)
        {
            Navigation?.GoLoginPage();
            return;
        }

        Context.CurrentUser = user;
        IsLoaded = true;
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
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
            if (UIConfig.AdminBody != null)
                UIConfig.AdminBody.Invoke(builder, BuildBody);
            else
                builder.Component<MainLayout>().Set(c => c.ChildContent, BuildBody).Build();
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