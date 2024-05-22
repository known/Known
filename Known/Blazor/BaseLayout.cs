namespace Known.Blazor;

public class BaseLayout : LayoutComponentBase
{
    //[Inject] protected AuthenticationStateProvider AuthProvider { get; set; }
    [Inject] private IHttpContextAccessor HttpAccessor { get; set; }
    [Inject] public NavigationManager Navigation { get; set; }
    [Inject] public JSService JS { get; set; }
    [CascadingParameter] public Context Context { get; set; }
    public Language Language => Context?.Language;
    public MenuInfo CurrentMenu => Context.Current;
    public HttpContext HttpContext => HttpAccessor?.HttpContext;

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
        Config.SetMenu(item);
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
        var user = Context?.CurrentUser;
        var result = await Platform.SignOutAsync(user?.Token);
        if (result.IsValid)
        {
            Context.CurrentUser = null;
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

    private Task SetCurrentUserAsync(UserInfo user)
    {
        HttpContext.User = null;
        return Task.CompletedTask;
        //if (AuthProvider is IAuthStateProvider provider)
        //{
        //    await provider.UpdateUserAsync(user);
        //}
    }
}