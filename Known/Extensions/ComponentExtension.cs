using Microsoft.AspNetCore.Components.CompilerServices;

namespace Known.Extensions;

public static class ComponentExtension
{
    public static void Cascading<T>(this RenderTreeBuilder builder, T value, RenderFragment child)
    {
        builder.Component<CascadingValue<T>>(attr =>
        {
            attr.Set(c => c.IsFixed, false)
                .Set(c => c.Value, value)
                .Set(c => c.ChildContent, child);
        });
    }

    public static ComponentBuilder<T> Component<T>(this RenderTreeBuilder builder, string id = null) where T : notnull, IBaseComponent
    {
        var comBuilder = new ComponentBuilder<T>(builder);
        comBuilder.Id(id);
        return comBuilder;
    }

    public static void Component<T>(this RenderTreeBuilder builder, Action<AttributeBuilder<T>> child) where T : notnull, IComponent
    {
        builder.OpenComponent<T>();
        var attr = new AttributeBuilder<T>(builder);
        child?.Invoke(attr);
        builder.AddMultipleAttributes(1, attr.Parameters);
        builder.CloseComponent();
    }

    public static void Component(this RenderTreeBuilder builder, Type type, Action<AttributeBuilder> child)
    {
        builder.OpenComponent(0, type);
        var attr = new AttributeBuilder(builder);
        child?.Invoke(attr);
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

    internal static void OpenComponent<T>(this RenderTreeBuilder builder) where T : notnull, IComponent
    {
        if (typeof(T).IsInterface)
        {
            var type = typeof(T);
            builder.OpenComponent(0, type);
        }
        else
        {
            builder.OpenComponent<T>(0);
        }
    }
}