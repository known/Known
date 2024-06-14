namespace Known.Extensions;

public static class ComponentExtension
{
    public static async Task<T> CreateAsync<T>(this IServiceScopeFactory factory)
    {
        await using var scope = factory.CreateAsyncScope();
        var service = scope.ServiceProvider.GetRequiredService<T>();
        return service;
    }

    public static async Task<T> CreateAsync<T>(this IServiceScopeFactory factory, Context context) where T : IService
    {
        await using var scope = factory.CreateAsyncScope();
        var service = scope.ServiceProvider.GetRequiredService<T>();
        service.Context = context;
        return service;
    }

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

    public static void Component(this RenderTreeBuilder builder, Type type, Dictionary<string, object> parameters = null)
    {
        builder.OpenComponent(0, type);
        if (parameters.Count > 0)
            builder.AddMultipleAttributes(1, parameters);
        builder.CloseComponent();
    }

    public static void DynamicComponent(this RenderTreeBuilder builder, Type type, Dictionary<string, object> parameters = null, Action<DynamicComponent> action = null)
    {
        if (type == null)
            return;

        builder.OpenComponent<DynamicComponent>(0);
        builder.AddAttribute(1, "Type", RuntimeHelpers.TypeCheck(type));
        builder.AddAttribute(1, "Parameters", parameters);
        builder.AddComponentReferenceCapture(2, value => action?.Invoke((DynamicComponent)value));
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