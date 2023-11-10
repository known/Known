using Known.Razor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.CompilerServices;
using Microsoft.AspNetCore.Components.Rendering;

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

    #region Component
    public static ComponentBuilder<T> Component<T>(this RenderTreeBuilder builder) where T : notnull, IComponent
    {
        return new ComponentBuilder<T>(builder);
    }

    internal static void Component<T>(this RenderTreeBuilder builder, Action<ComponentBuilder<T>> child) where T : notnull, IComponent
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

    #region Content
    public static void Fragment(this RenderTreeBuilder builder, RenderFragment fragment)
    {
        if (fragment != null)
            builder.AddContent(1, fragment);
    }

    public static void Fragment<TValue>(this RenderTreeBuilder builder, RenderFragment<TValue> fragment, TValue value)
    {
        if (fragment != null)
            builder.AddContent(1, fragment, value);
    }

    public static void Markup(this RenderTreeBuilder builder, string markup)
    {
        if (!string.IsNullOrWhiteSpace(markup))
            builder.AddMarkupContent(1, markup);
    }

    public static void Text(this RenderTreeBuilder builder, string text)
    {
        if (!string.IsNullOrWhiteSpace(text))
            builder.AddContent(1, text);
    }
    #endregion
}