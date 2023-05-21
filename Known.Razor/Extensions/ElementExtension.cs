using Microsoft.AspNetCore.Components.CompilerServices;

namespace Known.Razor.Extensions;

public static class ElementExtension
{
    public static RenderTreeBuilder Element(this RenderTreeBuilder builder, string name, Action<AttributeBuilder> child = null)
    {
        builder.OpenElement(0, name);
        var attr = new AttributeBuilder(builder);
        child?.Invoke(attr);
        builder.CloseElement();
        return builder;
    }

    public static ComponentBuilder<T> Component<T>(this RenderTreeBuilder builder) where T : notnull, BaseComponent => new ComponentBuilder<T>(builder);

    public static void Component<T>(this RenderTreeBuilder builder, Action<AttributeBuilder<T>> child) where T : notnull, IComponent
    {
        builder.OpenComponent<T>(0);
        var attr = new AttributeBuilder<T>(builder);
        child?.Invoke(attr);
        if (attr.Parameters.Count > 0)
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

    public static void DynamicComponent(this RenderTreeBuilder builder, Type type, Dictionary<string, object> parameters = null)
    {
        if (type == null)
            return;

        builder.OpenComponent<DynamicComponent>(0);
        builder.AddAttribute(1, "Type", RuntimeHelpers.TypeCheck(type));
        if (parameters != null)
            builder.AddAttribute(1, "Parameters", RuntimeHelpers.TypeCheck(parameters));
        builder.CloseComponent();
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
        if (!string.IsNullOrWhiteSpace(text))
            builder.AddContent(1, text);

        return builder;
    }

    public static RenderTreeBuilder IconName(this RenderTreeBuilder builder, string icon, string name)
    {
        if (!string.IsNullOrWhiteSpace(icon))
            builder.Icon(icon);
        builder.Span(name);
        return builder;
    }

    internal static RenderTreeBuilder IconName(this RenderTreeBuilder builder, string icon, string name, string nameStyle)
    {
        if (!string.IsNullOrWhiteSpace(icon))
            builder.Icon(icon);
        builder.Span(nameStyle, name);
        return builder;
    }
}