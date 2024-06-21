namespace Known.Blazor;

public class BaseLayout : LayoutComponentBase
{
    [Inject] protected IAuthStateProvider AuthProvider { get; set; }
    [Inject] public NavigationManager Navigation { get; set; }
    [Inject] public IServiceScopeFactory Factory { get; set; }
    [Inject] public JSService JS { get; set; }
    [CascadingParameter] public UIContext Context { get; set; }
    public Language Language => Context?.Language;
    public MenuInfo CurrentMenu => Context.Current;
    protected IAuthService AuthService { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await base.OnInitializedAsync();
            AuthService = await CreateServiceAsync<IAuthService>();
            await OnInitAsync();
        }
        catch (Exception ex)
        {
            await OnError(ex);
        }
    }

    protected virtual Task OnInitAsync() => Task.CompletedTask;

    public Task<T> CreateServiceAsync<T>() where T : IService => Factory.CreateAsync<T>(Context);
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
        var result = await AuthService.SignOutAsync(user?.Token);
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