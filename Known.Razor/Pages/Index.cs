namespace Known.Razor.Pages;

public class Index : BaseComponent
{
    private readonly string key = $"{Config.AppId}_User";
    private bool isLoaded;
    private bool isLogin;

    protected bool TopMenu { get; set; }

    protected override async Task OnInitializedAsync()
    {
        isLoaded = false;
        var result = await Platform.System.CheckInstallAsync();
        Context.Check = result.DataAs<CheckInfo>() ?? new CheckInfo();
        Context.CurrentUser = await GetCurrentUserAsync();
        isLogin = Context.CurrentUser != null;
        isLoaded = true;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!isLoaded)
            return;

        var isInstalled = Context.Check != null && Context.Check.IsInstalled;
        if (!isInstalled)
            BuildInstall(builder);
        else if (!isLogin)
            BuildLogin(builder);
        else
            BuildAdmin(builder);
    }

    protected virtual void BuildInstall(RenderTreeBuilder builder)
    {
        builder.Component<Install>().Set(c => c.OnInstall, OnInstall).Build();
    }

    protected virtual void BuildLogin(RenderTreeBuilder builder)
    {
        builder.Component<Login>().Set(c => c.OnLogin, OnLogin).Build();
    }

    protected virtual void BuildAdmin(RenderTreeBuilder builder)
    {
        builder.Component<Admin>().Set(c => c.OnLogout, OnLogout).Set(c => c.TopMenu, TopMenu).Build();
    }

    protected virtual Task<UserInfo> GetCurrentUserAsync()
    {
        return UI.GetSessionStorage<UserInfo>(key);
    }

    protected virtual Task UpdateCurrentUserAsync(UserInfo user)
    {
        return UI.SetSessionStorage(key, user);
    }

    protected void OnInstall(CheckInfo check)
    {
        Context.Check = check;
        StateChanged();
    }

    protected async void OnLogin(UserInfo user)
    {
        Context.CurrentUser = user;
        isLogin = Context.CurrentUser != null;
        await UpdateCurrentUserAsync(user);
        StateChanged();
    }

    protected async void OnLogout()
    {
        isLogin = false;
        await UpdateCurrentUserAsync(null);
        StateChanged();
    }
}