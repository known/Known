using Known.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

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
    [Inject] public NavigationManager Navigation { get; set; }
    [CascadingParameter] public Context Context { get; set; }
    [CascadingParameter] public Error Error { get; set; }

    protected bool IsMobile { get; private set; }
    protected bool IsLoaded { get; set; }
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

    protected override async Task OnInitializedAsync()
    {
        try
        {
            IsMobile = CheckMobile(HttpAccessor.HttpContext.Request);
            await base.OnInitializedAsync();
            await OnInitAsync();
        }
        catch (Exception ex)
        {
            if (Error != null)
                await Error.HandleAsync(ex);
        }
    }

    protected override async void BuildRenderTree(RenderTreeBuilder builder)
    {
        try
        {
            BuildRender(builder);
        }
        catch (Exception ex)
        {
            if (Error != null)
                await Error.HandleAsync(ex);
        }
    }

    protected virtual Task OnInitAsync() => Task.CompletedTask;
    protected virtual void BuildRender(RenderTreeBuilder builder) { }
    protected virtual ValueTask DisposeAsync(bool disposing) => ValueTask.CompletedTask;

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }

    public virtual void StateChanged() => InvokeAsync(StateHasChanged);

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
            log.Type = LogType.Page.ToString();

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
        if (menu.Tools != null && menu.Tools.Count > 0)
            hasButton = menu.Tools.Contains(buttonId);
        else if (menu.Actions != null && menu.Actions.Count > 0)
            hasButton = menu.Actions.Contains(buttonId);
        return hasButton;
    }

    private static bool CheckMobile(HttpRequest request)
    {
        var agent = request.Headers[HeaderNames.UserAgent].ToString();
        if (agent.Contains("Windows NT") || agent.Contains("Macintosh"))
            return false;

        bool flag = false;
        string[] keywords = ["Android", "iPhone", "iPod", "iPad", "Windows Phone", "MQQBrowser"];

        foreach (string item in keywords)
        {
            if (agent.Contains(item))
            {
                flag = true;
                break;
            }
        }

        return flag;
    }
}