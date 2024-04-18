using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;

namespace Known.Blazor;

public class LayoutPage : LayoutComponentBase
{
    [Inject] private IHttpContextAccessor HttpAccessor { get; set; }
    [Inject] public JSService JS { get; set; }
    [Inject] public NavigationManager Navigation { get; set; }
    [CascadingParameter] public Context Context { get; set; }

    public IUIService UI => Context?.UI;
    public Language Language => Context?.Language;
    public UserInfo CurrentUser => Context?.CurrentUser;
    public HttpContext HttpContext => HttpAccessor.HttpContext;

    private PlatformService platform;
    public PlatformService Platform
    {
        get
        {
            platform ??= new PlatformService(Context);
            return platform;
        }
    }

    protected AdminInfo Info { get; private set; }
    protected List<MenuItem> UserMenus { get; private set; }
    protected MenuItem CurrentMenu { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        CurrentMenu = Config.GetHomeMenu();
        if (CurrentUser != null)
        {
            Info = await Platform.Auth.GetAdminAsync();
            UserMenus = GetUserMenus(Info?.UserMenus);
            Context.UserSetting = Info?.UserSetting ?? new();
        }
    }

    protected async Task SignOutAsync()
    {
        var user = CurrentUser;
        var result = await Platform.SignOutAsync(user?.Token);
        if (result.IsValid)
        {
            Navigation.NavigateTo("/login");
            Config.OnExit?.Invoke();
        }
    }

    public virtual Task ShowSpinAsync(string text = null) => Task.CompletedTask;
    public virtual void HideSpin() { }

    protected virtual void RefreshPage() => StateHasChanged();

    private List<MenuItem> GetUserMenus(List<MenuInfo> menus)
    {
        Context.UserMenus = menus;
        return menus.ToMenuItems();
    }
}