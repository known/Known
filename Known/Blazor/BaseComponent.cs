namespace Known.Blazor;

public abstract class BaseComponent : ComponentBase, IAsyncDisposable
{
    public BaseComponent()
    {
        Id = Utils.GetGuid();
    }

    [Parameter] public string Id { get; set; }
    [Parameter] public string Name { get; set; }
    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public bool Enabled { get; set; } = true;
    [Parameter] public bool Visible { get; set; } = true;

    [Inject] private IHttpContextAccessor HttpAccessor { get; set; }
    [Inject] public JSService JS { get; set; }
    [Inject] public NavigationManager Navigation { get; set; }
    [CascadingParameter] public BaseLayout App { get; set; }

    protected bool IsDisposing { get; private set; }
    public Context Context => App?.Context;
    public IUIService UI => Context?.UI;
    public Language Language => App?.Language;
    public UserInfo CurrentUser => Context?.CurrentUser;
    public HttpContext HttpContext => HttpAccessor.HttpContext;
    public PlatformService Platform => App?.Platform;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await base.OnInitializedAsync();
            await OnInitAsync();
        }
        catch (Exception ex)
        {
            await App.OnError(ex);
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        try
        {
            await base.OnParametersSetAsync();
            await OnParameterAsync();
        }
        catch (Exception ex)
        {
            await App.OnError(ex);
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
            await App.OnError(ex);
        }
    }

    protected virtual Task OnInitAsync() => Task.CompletedTask;
    protected virtual Task OnParameterAsync() => Task.CompletedTask;
    protected virtual void BuildRender(RenderTreeBuilder builder) { }
    protected virtual ValueTask DisposeAsync(bool disposing)
    {
        IsDisposing = disposing;
        return ValueTask.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }

    public virtual void StateChanged() => InvokeAsync(StateHasChanged);

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
            await App.OnError(ex);
        }
    }
}