namespace Known.Blazor;

/// <summary>
/// 基础模板组件类。
/// </summary>
public class BaseLayout : LayoutComponentBase
{
    /// <summary>
    /// 取得或设置日志工厂实例。
    /// </summary>
    [Inject] public ILoggerFactory Logger { get; set; }

    /// <summary>
    /// 取得或设置注入的身份认证状态提供者实例。
    /// </summary>
    [Inject] protected IAuthStateProvider AuthProvider { get; set; }

    /// <summary>
    /// 取得或设置注入的导航管理者实例。
    /// </summary>
    [Inject] public NavigationManager Navigation { get; set; }

    /// <summary>
    /// 取得或设置注入的依赖注入服务工厂实例。
    /// </summary>
    [Inject] public IServiceScopeFactory Factory { get; set; }

    /// <summary>
    /// 取得或设置注入的JS服务实例。
    /// </summary>
    [Inject] public JSService JS { get; set; }

    /// <summary>
    /// 取得或设置注入的UI服务实例。
    /// </summary>
    [Inject] public UIService UI { get; set; }

    /// <summary>
    /// 取得或设置UI上下文对象级联值实例。
    /// </summary>
    [CascadingParameter] public UIContext Context { get; set; }

    /// <summary>
    /// 取得上下文语言对象实例。
    /// </summary>
    public Language Language => Context?.Language;

    /// <summary>
    /// 取得上下文当前菜单信息实例。
    /// </summary>
    public MenuInfo CurrentMenu => Context?.Current;

    /// <summary>
    /// 取得身份认证服务接口实例。
    /// </summary>
    internal IAuthService Auth { get; private set; }

    /// <summary>
    /// 取得系统服务接口实例。
    /// </summary>
    internal ISystemService System { get; private set; }

    /// <summary>
    /// 异步初始化模板，初始化UI多语言实例和上下文对象，以及全局异常处理；子模板不要覆写该方法，应覆写 OnInitAsync。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        try
        {
            await base.OnInitializedAsync();
            UI.Language = Language;
            Auth = await CreateServiceAsync<IAuthService>();
            System = await CreateServiceAsync<ISystemService>();
            if (Context.System == null)
                Context.System = await System.GetSystemAsync();
            await OnInitAsync();
        }
        catch (Exception ex)
        {
            await OnErrorAsync(ex);
        }
    }

    /// <summary>
    /// 模板呈现后异步操作方法，设置字体大小，语言，主题。
    /// </summary>
    /// <param name="firstRender">是否首次呈现。</param>
    /// <returns></returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            //JS不能在初始化中调用
            if (Config.App.IsSize)
            {
                var size = await JS.GetCurrentSizeAsync();
                if (string.IsNullOrWhiteSpace(size))
                    size = Context.UserSetting.Size;
                if (string.IsNullOrWhiteSpace(size))
                    size = Config.App.DefaultSize;
                await JS.SetCurrentSizeAsync(size);
            }
            if (Config.App.IsLanguage)
            {
                var language = await JS.GetCurrentLanguageAsync();
                if (string.IsNullOrWhiteSpace(language))
                    language = Context.UserSetting.Language;
                Context.CurrentLanguage = language;
            }
        }
    }

    /// <summary>
    /// 异步初始化模板虚方法，子模板应覆写该方法。
    /// </summary>
    /// <returns></returns>
    protected virtual Task OnInitAsync() => Task.CompletedTask;

    /// <summary>
    /// 创建依赖注入的后端服务接口实例。
    /// </summary>
    /// <typeparam name="T">继承 IService 的服务接口。</typeparam>
    /// <returns>后端服务接口实例。</returns>
    public Task<T> CreateServiceAsync<T>() where T : IService => Factory.CreateAsync<T>(Context);

    /// <summary>
    /// 导航到指定菜单对应的页面。
    /// </summary>
    /// <param name="item">跳转的菜单对象。</param>
    public void NavigateTo(MenuInfo item) => Navigation?.NavigateTo(item);

    /// <summary>
    /// 返回到上一个页面。
    /// </summary>
    public void Back()
    {
        if (CurrentMenu == null || string.IsNullOrWhiteSpace(CurrentMenu.BackUrl))
            return;

        Navigation?.NavigateTo(CurrentMenu.BackUrl);
    }

    /// <summary>
    /// 异步注销，用户安全退出系统。
    /// </summary>
    /// <returns></returns>
    public async Task SignOutAsync()
    {
        var user = await AuthProvider.GetUserAsync();
        var result = await Auth.SignOutAsync();
        if (result.IsValid)
        {
            Context.SignOut();
            await SetCurrentUserAsync(null);
            Navigation?.GoLoginPage();
            Config.OnExit?.Invoke();
        }
    }

    /// <summary>
    /// 全局异常处理方法。
    /// </summary>
    /// <param name="ex">异常信息。</param>
    /// <returns></returns>
    public async Task OnErrorAsync(Exception ex)
    {
        Logger.CreateLogger<BaseLayout>().Error(ex);
        var message = ex.Message;
#if DEBUG
        message = ex.ToString();
#endif
        await UI.NoticeAsync(message, StyleType.Error);
    }

    /// <summary>
    /// 异步显示加载提示框虚方法。
    /// </summary>
    /// <param name="text">加载提示信息。</param>
    /// <param name="action">异步加载方法的委托。</param>
    /// <returns></returns>
    public virtual Task ShowSpinAsync(string text, Func<Task> action) => Task.CompletedTask;

    /// <summary>
    /// 异步通知组件状态改变，重新呈现组件。
    /// </summary>
    /// <returns></returns>
    public virtual Task StateChangedAsync() => InvokeAsync(StateHasChanged);

    /// <summary>
    /// 异步设置当前登录用户信息。
    /// </summary>
    /// <param name="user">用户信息。</param>
    /// <returns></returns>
    public async Task SetCurrentUserAsync(UserInfo user)
    {
        if (AuthProvider != null)
            await AuthProvider.SignInAsync(user);
    }
}

/// <summary>
/// 空模板组件类。
/// </summary>
public class EmptyLayout : BaseLayout
{
    /// <summary>
    /// 呈现空模板组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<KLayout>()
               .Set(c => c.Layout, this)
               .Set(c => c.ChildContent, Body)
               .Build();
    }
}