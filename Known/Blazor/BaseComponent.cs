using Known.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;

namespace Known.Blazor;

public abstract class BaseComponent : ComponentBase, IAsyncDisposable
{
    private readonly Type type;

    public BaseComponent()
    {
        type = GetType();
        Id = type.Name;
    }

    [Parameter] public string Id { get; set; }
    [Parameter] public string Name { get; set; }
    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public bool Enabled { get; set; } = true;
    [Parameter] public bool Visible { get; set; } = true;

    [Inject] private IHttpContextAccessor HttpAccessor { get; set; }
    [Inject] public JSService JS { get; set; }
    [CascadingParameter] public Context Context { get; set; }

    protected bool IsLoaded { get; set; }
    public IUIService UI => Context?.UI;
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

    protected virtual ValueTask DisposeAsync(bool disposing) => ValueTask.CompletedTask;

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }

    public void StateChanged() => InvokeAsync(StateHasChanged);

    protected bool HasButton(string buttonId)
    {
        var user = CurrentUser;
        if (user == null)
            return false;

        if (user.IsAdmin)
            return true;

        return IsInMenu(Id, buttonId);
    }

    internal async Task AddVisitLogAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
            return;

        var log = new SysLog { Target = Name, Content = type.FullName };
        if (Context.UserMenus != null && Context.UserMenus.Exists(p => p.Code == type.Name))
            log.Type = LogType.Page;

        if (string.IsNullOrWhiteSpace(log.Type))
            return;

        await Platform.System.AddLogAsync(log);
    }

    private bool IsInMenu(string pageId, string buttonId)
    {
        var menu = Context.UserMenus.FirstOrDefault(m => m.Id == pageId || m.Code == pageId);
        if (menu == null)
            return false;

        var hasButton = false;
        if (menu.Buttons != null && menu.Buttons.Count > 0)
            hasButton = menu.Buttons.Contains(buttonId);
        else if (menu.Actions != null && menu.Actions.Count > 0)
            hasButton = menu.Actions.Contains(buttonId);
        return hasButton;
    }
}