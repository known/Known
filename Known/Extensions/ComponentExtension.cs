namespace Known.Extensions;

/// <summary>
/// 组件呈现扩展类。
/// </summary>
public static class ComponentExtension
{
    /// <summary>
    /// 创建依赖注入的接口实例。
    /// </summary>
    /// <typeparam name="T">接口类型。</typeparam>
    /// <param name="factory">依赖注入服务工厂实例。</param>
    /// <returns></returns>
    public static async Task<T> CreateAsync<T>(this IServiceScopeFactory factory)
    {
        await using var scope = factory.CreateAsyncScope();
        var service = scope.ServiceProvider.GetRequiredService<T>();
        return service;
    }

    /// <summary>
    /// 创建依赖注入的后端服务接口实例。
    /// </summary>
    /// <typeparam name="T">继承 IService 的服务接口。</typeparam>
    /// <param name="factory">依赖注入服务工厂实例。</param>
    /// <param name="context">上下文对象实例。</param>
    /// <returns></returns>
    public static async Task<T> CreateAsync<T>(this IServiceScopeFactory factory, Context context) where T : IService
    {
        await using var scope = factory.CreateAsyncScope();
        var service = scope.ServiceProvider.GetRequiredService<T>();
        service.Context = context;
        return service;
    }

    /// <summary>
    /// 构建级联值组件。
    /// </summary>
    /// <typeparam name="T">级联组件类型。</typeparam>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="value">级联组件对象。</param>
    /// <param name="child">级联组件子内容。</param>
    public static void Cascading<T>(this RenderTreeBuilder builder, T value, RenderFragment child)
    {
        builder.Component<CascadingValue<T>>(attr =>
        {
            attr.Set(c => c.IsFixed, true)
                .Set(c => c.Value, value)
                .Set(c => c.ChildContent, child);
        });
    }

    #region Component
    /// <summary>
    /// 创建组件建造者。
    /// </summary>
    /// <typeparam name="T">组件类型。</typeparam>
    /// <param name="builder">呈现树建造者。</param>
    /// <returns>组件建造者。</returns>
    public static ComponentBuilder<T> Component<T>(this RenderTreeBuilder builder) where T : notnull, Microsoft.AspNetCore.Components.IComponent
    {
        return new ComponentBuilder<T>(builder);
    }

    internal static void Component<T>(this RenderTreeBuilder builder, Action<ComponentBuilder<T>> child) where T : notnull, Microsoft.AspNetCore.Components.IComponent
    {
        var attr = new ComponentBuilder<T>(builder);
        child?.Invoke(attr);
        attr.Build();
    }

    /// <summary>
    /// 构建组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="type">组件类型。</param>
    /// <param name="parameters">组件参数。</param>
    /// <param name="action">组件实例委托。</param>
    public static void Component(this RenderTreeBuilder builder, Type type, Dictionary<string, object> parameters = null, Action<object> action = null)
    {
        builder.OpenComponent(0, type);
        if (parameters != null && parameters.Count > 0)
            builder.AddMultipleAttributes(1, parameters);
        if (action != null)
            builder.AddComponentReferenceCapture(2, value => action.Invoke(value));
        builder.CloseComponent();
    }

    /// <summary>
    /// 构建动态组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="type">组件类型。</param>
    /// <param name="parameters">组件参数。</param>
    /// <param name="action">组件实例委托。</param>
    public static void DynamicComponent(this RenderTreeBuilder builder, Type type, Dictionary<string, object> parameters = null, Action<DynamicComponent> action = null)
    {
        if (type == null)
            return;

        builder.OpenComponent<DynamicComponent>(0);
        builder.AddAttribute(1, "Type", RuntimeHelpers.TypeCheck(type));
        if (parameters != null && parameters.Count > 0)
            builder.AddAttribute(2, "Parameters", parameters);
        if (action != null)
            builder.AddComponentReferenceCapture(3, value => action.Invoke((DynamicComponent)value));
        builder.CloseComponent();
    }
    #endregion

    #region Callback
    /// <summary>
    /// 创建事件回调。
    /// </summary>
    /// <param name="component">组件对象。</param>
    /// <param name="callback">回调异步委托。</param>
    /// <returns>事件回调。</returns>
    public static EventCallback Callback(this ComponentBase component, Func<Task> callback) => EventCallback.Factory.Create(component, callback);

    /// <summary>
    /// 创建事件回调。
    /// </summary>
    /// <param name="component">组件对象。</param>
    /// <param name="callback">回调委托。</param>
    /// <returns>事件回调。</returns>
    public static EventCallback Callback(this ComponentBase component, Action callback) => EventCallback.Factory.Create(component, callback);

    /// <summary>
    /// 创建事件回调。
    /// </summary>
    /// <typeparam name="T">组件类型。</typeparam>
    /// <param name="component">组件对象。</param>
    /// <param name="callback">回调委托。</param>
    /// <returns>事件回调。</returns>
    public static EventCallback<T> Callback<T>(this ComponentBase component, Action<T> callback) => EventCallback.Factory.Create(component, callback);
    #endregion

    #region Content
    /// <summary>
    /// 建造组件树。
    /// </summary>
    /// <param name="component">组件对象。</param>
    /// <param name="action">建造委托。</param>
    /// <returns>UI内容。</returns>
    public static RenderFragment BuildTree(this ComponentBase component, Action<RenderTreeBuilder> action)
    {
        return delegate (RenderTreeBuilder builder) { action(builder); };
    }

    /// <summary>
    /// 建造组件树。
    /// </summary>
    /// <typeparam name="T">对象类型。</typeparam>
    /// <param name="component">组件对象。</param>
    /// <param name="action">建造委托。</param>
    /// <returns>对象UI内容。</returns>
    public static RenderFragment<T> BuildTree<T>(this ComponentBase component, Action<RenderTreeBuilder, T> action)
    {
        return row => delegate (RenderTreeBuilder builder) { action(builder, row); };
    }

    /// <summary>
    /// 呈现一个片段。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="fragment">组件片段。</param>
    /// <returns>呈现树建造者。</returns>
    public static RenderTreeBuilder Fragment(this RenderTreeBuilder builder, RenderFragment fragment)
    {
        if (fragment != null)
            builder.AddContent(1, fragment);
        return builder;
    }

    /// <summary>
    /// 呈现一个带数据对象的片段。
    /// </summary>
    /// <typeparam name="TValue">对象类型。</typeparam>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="fragment">组件片段。</param>
    /// <param name="value">数据对象。</param>
    /// <returns>呈现树建造者。</returns>
    public static RenderTreeBuilder Fragment<TValue>(this RenderTreeBuilder builder, RenderFragment<TValue> fragment, TValue value)
    {
        if (fragment != null)
            builder.AddContent(1, fragment, value);
        return builder;
    }

    /// <summary>
    /// 呈现HTML字符串内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="markup">HTML字符串。</param>
    /// <returns>呈现树建造者。</returns>
    public static RenderTreeBuilder Markup(this RenderTreeBuilder builder, string markup)
    {
        if (!string.IsNullOrWhiteSpace(markup))
            builder.AddMarkupContent(1, markup);
        return builder;
    }

    /// <summary>
    /// 呈现一个文本内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="text">文本内容。</param>
    /// <returns>呈现树建造者。</returns>
    public static RenderTreeBuilder Text(this RenderTreeBuilder builder, string text)
    {
        builder.AddContent(1, text);
        return builder;
    }
    #endregion
}