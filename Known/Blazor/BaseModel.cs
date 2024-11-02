namespace Known.Blazor;

/// <summary>
/// 抽象组件模型基类。
/// </summary>
/// <param name="context">UI上下文对象。</param>
public abstract class BaseModel(UIContext context)
{
    /// <summary>
    /// 取得UI上下文对象实例。
    /// </summary>
    public UIContext Context { get; } = context;

    /// <summary>
    /// 取得或设置注入的抽象UI服务实例。
    /// </summary>
    public IUIService UI => Context?.UI;

    /// <summary>
    /// 取得上下文语言对象实例。
    /// </summary>
    public Language Language => Context?.Language;

    /// <summary>
    /// 取得上下文当前用户信息实例。
    /// </summary>
    public UserInfo CurrentUser => Context?.CurrentUser;

    /// <summary>
    /// 取得或设置组件状态改变方法委托。
    /// </summary>
    public Action OnStateChanged { get; set; }

    /// <summary>
    /// 取得或设置组件状态改变方法委托。
    /// </summary>
    public Func<Task> OnStateChangedTask { get; set; }

    /// <summary>
    /// 改变组件状态。
    /// </summary>
    public void StateChange() => OnStateChanged?.Invoke();

    /// <summary>
    /// 异步改变组件状态。
    /// </summary>
    public Task StateChangeAsync() => OnStateChangedTask?.Invoke();
}