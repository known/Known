﻿namespace Known.Blazor;

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

    [Inject] public IJSRuntime JSRuntime { get; set; }
    [Inject] public JSService JS { get; set; }
    [Inject] public IUIService UI { get; set; }
    [Inject] public NavigationManager Navigation { get; set; }
    [Inject] public IServiceScopeFactory Factory { get; set; }
    [CascadingParameter] public UIContext Context { get; set; }
    [CascadingParameter] public BaseLayout App { get; set; }

    protected bool IsDisposing { get; private set; }
    public Language Language => Context?.Language;
    public UserInfo CurrentUser => Context?.CurrentUser;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await base.OnInitializedAsync();
            UI.Language = Language;
            Context.UI = UI;
            await OnInitAsync();
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(ex);
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
            await HandleExceptionAsync(ex);
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
            await HandleExceptionAsync(ex);
        }
    }

    protected virtual Task OnInitAsync() => Task.CompletedTask;
    protected virtual Task OnParameterAsync() => Task.CompletedTask;
    protected virtual Task OnDisposeAsync() => Task.CompletedTask;
    protected virtual void BuildRender(RenderTreeBuilder builder) { }
    
    public Task<T> CreateServiceAsync<T>() where T : IService => Factory.CreateAsync<T>(Context);
    public virtual Task RefreshAsync() => Task.CompletedTask;
    public void StateChanged() => StateHasChanged();
    public Task StateChangedAsync() => InvokeAsync(StateHasChanged);
    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }

    internal void OnToolClick(ActionInfo info) => OnAction(info, null);
    internal void OnActionClick<TModel>(ActionInfo info, TModel item) => OnAction(info, [item]);
    internal async void OnAction(ActionInfo info, object[] parameters)
    {
        await TypeHelper.ActionAsync(this, Context, App, info, parameters);
    }

    private async ValueTask DisposeAsync(bool disposing)
    {
        IsDisposing = disposing;
        await OnDisposeAsync();
    }

    private async Task HandleExceptionAsync(Exception ex)
    {
        if (App != null)
            await App.OnError(ex);
    }
}