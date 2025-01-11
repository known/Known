namespace Known.Blazor;

/// <summary>
/// 抽象组件基类。
/// </summary>
public abstract class BaseComponent : ComponentBase, IAsyncDisposable
{
    /// <summary>
    /// 构造函数，创建一个组件实例，默认组件ID。
    /// </summary>
    public BaseComponent()
    {
        Id = Utils.GetNextId();
    }

    /// <summary>
    /// 取得或设置是否是静态组件。
    /// </summary>
    [Parameter] public bool IsStatic { get; set; } = true;

    /// <summary>
    /// 取得或设置组件ID。
    /// </summary>
    [Parameter] public string Id { get; set; }

    /// <summary>
    /// 取得或设置组件名称。
    /// </summary>
    [Parameter] public string Name { get; set; }

    /// <summary>
    /// 取得或设置组件是否只读。
    /// </summary>
    [Parameter] public bool ReadOnly { get; set; }

    /// <summary>
    /// 取得或设置组件是否可用，默认可用。
    /// </summary>
    [Parameter] public bool Enabled { get; set; } = true;

    /// <summary>
    /// 取得或设置组件是否可见，默认可见。
    /// </summary>
    [Parameter] public bool Visible { get; set; } = true;

    /// <summary>
    /// 取得或设置日志工厂实例。
    /// </summary>
    [Inject] public ILoggerFactory Logger { get; set; }

    /// <summary>
    /// 取得或设置JS运行时实例。
    /// </summary>
    [Inject] public IJSRuntime JSRuntime { get; set; }

    /// <summary>
    /// 取得或设置注入的JS服务实例。
    /// </summary>
    [Inject] public JSService JS { get; set; }

    /// <summary>
    /// 取得或设置注入的UI服务实例。
    /// </summary>
    [Inject] public UIService UI { get; set; }

    /// <summary>
    /// 取得或设置注入的导航管理者实例。
    /// </summary>
    [Inject] public NavigationManager Navigation { get; set; }

    /// <summary>
    /// 取得或设置注入的依赖注入服务工厂实例。
    /// </summary>
    [Inject] private IServiceScopeFactory Factory { get; set; }

    /// <summary>
    /// 取得或设置UI上下文对象级联值实例。
    /// </summary>
    [CascadingParameter] public UIContext Context { get; set; }

    /// <summary>
    /// 取得或设置基础模板布局级联值实例。
    /// </summary>
    [CascadingParameter] public BaseLayout App { get; set; }

    /// <summary>
    /// 取得组件是否是Server模式。
    /// </summary>
    public bool IsServerMode => JSRuntime.GetType().ToString() == "Microsoft.AspNetCore.Components.Server.Circuits.RemoteJSRuntime";

    /// <summary>
    /// 取得上下文语言对象实例。
    /// </summary>
    public Language Language => Context?.Language;

    /// <summary>
    /// 取得上下文当前用户信息实例。
    /// </summary>
    public UserInfo CurrentUser => Context?.CurrentUser;

    /// <summary>
    /// 取得框架平台数据服务接口实例。
    /// </summary>
    public IPlatformService Platform { get; private set; }

    /// <summary>
    /// 取得框架管理后台数据服务接口实例。
    /// </summary>
    public IAdminService Admin { get; private set; }

    /// <summary>
    /// 取得是否释放组件对象。
    /// </summary>
    protected bool IsDisposing { get; private set; }

    /// <summary>
    /// 异步初始化组件，初始化UI多语言实例和上下文对象，以及全局异常处理；子组件不要覆写该方法，应覆写 OnInitAsync。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        try
        {
            await base.OnInitializedAsync();
            UI.Language = Language;
            Platform = await CreateServiceAsync<IPlatformService>();
            Admin = await CreateServiceAsync<IAdminService>();
            await OnInitAsync();
        }
        catch (Exception ex)
        {
            await OnErrorAsync(ex);
        }
    }

    /// <summary>
    /// 异步设置组件参数，以及全局异常处理；子组件不要覆写该方法，应覆写 OnParameterAsync。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnParametersSetAsync()
    {
        try
        {
            await base.OnParametersSetAsync();
            await OnParameterAsync();
        }
        catch (Exception ex)
        {
            await OnErrorAsync(ex);
        }
    }

    /// <summary>
    /// 组件呈现后执行的方法，可判断组件是否是静态组件。
    /// 静态组件不执行该方法，交互式组件会执行该方法。
    /// </summary>
    /// <param name="firstRender">是否首次呈现。</param>
    /// <returns></returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        IsStatic = false;
        await base.OnAfterRenderAsync(firstRender);
    }

    /// <summary>
    /// 呈现组件内容，以及全局异常处理；子组件不要覆写该方法，应覆写 BuildRender。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override async void BuildRenderTree(RenderTreeBuilder builder)
    {
        try
        {
            BuildRender(builder);
        }
        catch (Exception ex)
        {
            await OnErrorAsync(ex);
        }
    }

    /// <summary>
    /// 异步初始化组件。
    /// </summary>
    /// <returns></returns>
    protected virtual Task OnInitAsync() => Task.CompletedTask;

    /// <summary>
    /// 异步设置组件参数。
    /// </summary>
    /// <returns></returns>
    protected virtual Task OnParameterAsync() => Task.CompletedTask;

    /// <summary>
    /// 异步释放组件实例。
    /// </summary>
    /// <returns></returns>
    protected virtual Task OnDisposeAsync() => Task.CompletedTask;

    /// <summary>
    /// 呈现组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected virtual void BuildRender(RenderTreeBuilder builder) { }

    /// <summary>
    /// 创建依赖注入的后端服务接口实例。
    /// </summary>
    /// <typeparam name="T">继承 IService 的服务接口。</typeparam>
    /// <returns></returns>
    public Task<T> CreateServiceAsync<T>() where T : IService => Factory.CreateAsync<T>(Context);

    /// <summary>
    /// 异步刷新组件。
    /// </summary>
    /// <returns></returns>
    public virtual Task RefreshAsync() => Task.CompletedTask;

    /// <summary>
    /// 通知组件状态改变，重新呈现组件。
    /// </summary>
    public void StateChanged() => StateHasChanged();

    /// <summary>
    /// 异步通知组件状态改变，重新呈现组件。
    /// </summary>
    /// <returns></returns>
    public Task StateChangedAsync() => InvokeAsync(StateHasChanged);

    /// <summary>
    /// 全局异常处理方法。
    /// </summary>
    /// <param name="ex">异常信息。</param>
    /// <returns></returns>
    public async Task OnErrorAsync(Exception ex)
    {
        Logger.CreateLogger<BaseComponent>().Error(ex);
        var message = Config.IsDebug ? ex.ToString() : ex.Message;
        await UI.NoticeAsync(Language?["Title.Error"], message, StyleType.Error);
    }

    /// <summary>
    /// 异步释放组件对象实例。
    /// </summary>
    /// <returns></returns>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }

    internal void OnToolClick(ActionInfo info) => OnAction(info, null);
    internal void OnActionClick<TModel>(ActionInfo info, TModel item) => OnAction(info, [item]);
    internal void OnAction(ActionInfo info, object[] parameters)
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
            UI.Error(ex.Message);
        }
    }

    private async ValueTask DisposeAsync(bool disposing)
    {
        IsDisposing = disposing;
        await OnDisposeAsync();
    }
}