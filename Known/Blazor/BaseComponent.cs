using Known.Entities;
using Known.Extensions;
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
    [CascadingParameter] public KError Error { get; set; }
    [CascadingParameter] public AdminPage Admin { get; set; }

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
            IsMobile = CheckMobile(HttpAccessor?.HttpContext?.Request);
            await base.OnInitializedAsync();
            await OnInitAsync();
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
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
            Logger.Exception(ex);
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

    internal async void OnAction(ActionInfo info, object[] parameters)
    {
        var type = GetType();
        var paramTypes = parameters?.Select(p => p.GetType()).ToArray();
        var method = paramTypes == null
                   ? type.GetMethod(info.Id)
                   : type.GetMethod(info.Id, paramTypes);
        if (method == null)
        {
            var message = Language["Tip.NoMethod"].Replace("{method}", $"{info.Name}[{type.Name}.{info.Id}]");
            UI.Error(message);
            return;
        }

        try
        {
            method.Invoke(this, parameters);
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
            if (Error != null)
                await Error.HandleAsync(ex);
        }
    }

    private static bool CheckMobile(HttpRequest request)
    {
        if (request == null)
            return false;

        var agent = request.Headers[HeaderNames.UserAgent].ToString();
        return Utils.CheckMobile(agent);
    }
}