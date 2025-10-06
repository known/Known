namespace Known.Blazor;

/// <summary>
/// 抽象组件模型基类。
/// </summary>
/// <param name="component">模型关联的组件对象。</param>
public abstract class BaseModel(IBaseComponent component)
{
    /// <summary>
    /// 取得模型关联的组件对象。
    /// </summary>
    public IBaseComponent Component { get; } = component;

    /// <summary>
    /// 取得UI上下文对象实例。
    /// </summary>
    public UIContext Context => Component?.Context;

    /// <summary>
    /// 取得注入的UI服务实例。
    /// </summary>
    public UIService UI => Component?.UI;

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
    internal Action OnStateChanged { get; set; }

    /// <summary>
    /// 取得或设置组件状态改变方法委托。
    /// </summary>
    internal Func<Task> OnStateChangedTask { get; set; }

    /// <summary>
    /// 改变组件状态。
    /// </summary>
    public void StateChanged() => OnStateChanged?.Invoke();

    /// <summary>
    /// 异步改变组件状态。
    /// </summary>
    public Task StateChangedAsync() => OnStateChangedTask?.Invoke();
}