namespace Known.Extensions;

/// <summary>
/// 组件扩展类。
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

    public static void Component(this RenderTreeBuilder builder, Type type, Dictionary<string, object> parameters = null, Action<object> action = null)
    {
        builder.OpenComponent(0, type);
        if (parameters != null && parameters.Count > 0)
            builder.AddMultipleAttributes(1, parameters);
        if (action != null)
            builder.AddComponentReferenceCapture(2, value => action.Invoke(value));
        builder.CloseComponent();
    }

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
    public static EventCallback Callback(this ComponentBase component, Func<Task> callback) => EventCallback.Factory.Create(component, callback);
    public static EventCallback Callback(this ComponentBase component, Action callback) => EventCallback.Factory.Create(component, callback);
    public static EventCallback<T> Callback<T>(this ComponentBase component, Action<T> callback) => EventCallback.Factory.Create(component, callback);
    #endregion

    #region Content
    public static RenderFragment BuildTree(this ComponentBase component, Action<RenderTreeBuilder> action)
    {
        return delegate (RenderTreeBuilder builder) { action(builder); };
    }

    public static RenderFragment<T> BuildTree<T>(this ComponentBase component, Action<RenderTreeBuilder, T> action)
    {
        return row => delegate (RenderTreeBuilder builder) { action(builder, row); };
    }

    public static RenderTreeBuilder Fragment(this RenderTreeBuilder builder, RenderFragment fragment)
    {
        if (fragment != null)
            builder.AddContent(1, fragment);
        return builder;
    }

    public static RenderTreeBuilder Fragment<TValue>(this RenderTreeBuilder builder, RenderFragment<TValue> fragment, TValue value)
    {
        if (fragment != null)
            builder.AddContent(1, fragment, value);
        return builder;
    }

    public static RenderTreeBuilder Markup(this RenderTreeBuilder builder, string markup)
    {
        if (!string.IsNullOrWhiteSpace(markup))
            builder.AddMarkupContent(1, markup);
        return builder;
    }

    public static RenderTreeBuilder Text(this RenderTreeBuilder builder, string text)
    {
        builder.AddContent(1, text);
        return builder;
    }
    #endregion
}