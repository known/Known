namespace Known.Extensions;

/// <summary>
/// 组件回调事件扩展类。
/// </summary>
public static class CallbackExtension
{
    /// <summary>
    /// 创建事件回调。
    /// </summary>
    /// <param name="component">组件对象。</param>
    /// <param name="callback">回调异步委托。</param>
    /// <returns>事件回调。</returns>
    public static EventCallback Callback(this Microsoft.AspNetCore.Components.IComponent component, Func<Task> callback)
    {
        if (component == null)
            return EventCallback.Empty;

        return EventCallback.Factory.Create(component, callback);
    }

    /// <summary>
    /// 创建事件回调。
    /// </summary>
    /// <typeparam name="T">参数类型。</typeparam>
    /// <param name="component">组件对象。</param>
    /// <param name="callback">回调异步委托。</param>
    /// <returns>事件回调。</returns>
    public static EventCallback<T> Callback<T>(this Microsoft.AspNetCore.Components.IComponent component, Func<T, Task> callback)
    {
        if (component == null)
            return EventCallback<T>.Empty;

        return EventCallback.Factory.Create(component, callback);
    }

    /// <summary>
    /// 创建事件回调。
    /// </summary>
    /// <param name="component">组件对象。</param>
    /// <param name="callback">回调委托。</param>
    /// <returns>事件回调。</returns>
    public static EventCallback Callback(this Microsoft.AspNetCore.Components.IComponent component, Action callback)
    {
        if (component == null)
            return EventCallback.Empty;

        return EventCallback.Factory.Create(component, callback);
    }

    /// <summary>
    /// 创建事件回调。
    /// </summary>
    /// <typeparam name="T">参数类型。</typeparam>
    /// <param name="component">组件对象。</param>
    /// <param name="callback">回调委托。</param>
    /// <returns>事件回调。</returns>
    public static EventCallback<T> Callback<T>(this Microsoft.AspNetCore.Components.IComponent component, Action<T> callback)
    {
        if (component == null)
            return EventCallback<T>.Empty;

        return EventCallback.Factory.Create(component, callback);
    }
}