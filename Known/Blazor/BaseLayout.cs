namespace Known.Blazor;

public class BaseLayout : LayoutComponentBase
{
    [Inject] protected IAuthStateProvider AuthProvider { get; set; }
    [Inject] public NavigationManager Navigation { get; set; }
    [Inject] public JSService JS { get; set; }
    [CascadingParameter] public UIContext Context { get; set; }
    public Language Language => Context?.Language;
    public MenuInfo CurrentMenu => Context.Current;

    private PlatformService platform;
    public PlatformService Platform
    {
        get
        {
            platform ??= new PlatformService(Context);
            return platform;
        }
    }

    public void NavigateTo(string url) => Navigation.NavigateTo(url);

    public void NavigateTo(MenuInfo item)
    {
        if (item == null)
            return;

        //缓存APP代码中添加的菜单
        UIConfig.SetMenu(item);
        NavigateTo(item.RouteUrl);
    }

    public void Back()
    {
        if (CurrentMenu == null || string.IsNullOrWhiteSpace(CurrentMenu.BackUrl))
            return;

        NavigateTo(CurrentMenu.BackUrl);
    }

    public async Task SignOutAsync()
    {
        var user = await AuthProvider.GetUserAsync();
        var result = await Platform.Auth.SignOutAsync(user?.Token);
        if (result.IsValid)
        {
            Context.SignOut();
            await SetCurrentUserAsync(null);
            NavigateTo("/login");
            Config.OnExit?.Invoke();
        }
    }

    public async Task OnError(Exception ex)
    {
        Logger.Exception(ex);
        await Context.UI.Notice(ex.Message, StyleType.Error);
    }

    public virtual Task ShowSpinAsync(string text, Action action) => Task.CompletedTask;
    public virtual void StateChanged() => InvokeAsync(StateHasChanged);

    private async Task SetCurrentUserAsync(UserInfo user)
    {
        if (AuthProvider != null)
            await AuthProvider.SetUserAsync(user);
    }
}