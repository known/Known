namespace Known.Razor;

public class AttributeBuilder<T> where T : IComponent
{
    private readonly RenderTreeBuilder builder;
    internal readonly Dictionary<string, object> Parameters = new(StringComparer.Ordinal);

    public AttributeBuilder(RenderTreeBuilder builder)
    {
        this.builder = builder;
    }

    public AttributeBuilder<T> Add(string name, object value)
    {
        Parameters[name] = value;
        return this;
    }

    public AttributeBuilder<T> Set<TValue>(Expression<Func<T, TValue>> selector, TValue value)
    {
        var property = TypeHelper.Property(selector);
        Parameters[property.Name] = value;
        return this;
    }

    public void Build()
    {
        builder.OpenComponent<T>(0);
        if (Parameters.Count > 0)
            builder.AddMultipleAttributes(1, Parameters);
        builder.CloseComponent();
    }

    public void Build(Action<T> action)
    {
        builder.OpenComponent<T>(0);
        if (Parameters.Count > 0)
            builder.AddMultipleAttributes(1, Parameters);
        builder.AddComponentReferenceCapture(2, value => action?.Invoke((T)value));
        builder.CloseComponent();
    }
}

public class AttributeBuilder
{
    private readonly RenderTreeBuilder builder;

    public AttributeBuilder(RenderTreeBuilder builder)
    {
        this.builder = builder;
    }

    public AttributeBuilder Id(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            builder.AddAttribute(1, "id", value);

        return this;
    }

    public AttributeBuilder Name(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            builder.AddAttribute(1, "name", value);

        return this;
    }

    public AttributeBuilder For(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            builder.AddAttribute(1, "for", value);

        return this;
    }

    public AttributeBuilder Role(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            builder.AddAttribute(1, "role", value);

        return this;
    }

    public AttributeBuilder Class(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            builder.AddAttribute(1, "class", value);

        return this;
    }

    public AttributeBuilder Style(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            builder.AddAttribute(1, "style", value);

        return this;
    }

    public AttributeBuilder Title(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            builder.AddAttribute(1, "title", value);

        return this;
    }

    public AttributeBuilder Placeholder(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            builder.AddAttribute(1, "placeholder", value);

        return this;
    }

    public AttributeBuilder Type(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            builder.AddAttribute(1, "type", value);

        return this;
    }

    public AttributeBuilder Value(string value)
    {
        builder.AddAttribute(1, "value", value);
        return this;
    }

    public AttributeBuilder Src(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            builder.AddAttribute(1, "src", value);

        return this;
    }

    public AttributeBuilder RowSpan(int rowSpan)
    {
        if (rowSpan > 0)
            builder.AddAttribute(1, "rowspan", rowSpan);

        return this;
    }

    public AttributeBuilder ColSpan(int colSpan)
    {
        if (colSpan > 0)
            builder.AddAttribute(1, "colspan", colSpan);

        return this;
    }

    public AttributeBuilder Required(bool value)
    {
        builder.AddAttribute(1, "required", value);
        return this;
    }

    public AttributeBuilder Readonly(bool value)
    {
        builder.AddAttribute(1, "readonly", value);
        return this;
    }

    public AttributeBuilder Disabled(bool value)
    {
        builder.AddAttribute(1, "disabled", value);
        return this;
    }

    public AttributeBuilder Checked(bool value)
    {
        builder.AddAttribute(1, "checked", value);
        return this;
    }

    public AttributeBuilder Selected(bool value)
    {
        builder.AddAttribute(1, "selected", value);
        return this;
    }

    public AttributeBuilder OnClick(string onClick)
    {
        if (onClick != null)
            builder.AddAttribute(1, "onclick", onClick);

        return this;
    }

    public AttributeBuilder OnClick(EventCallback? onClick, bool stopPropagation = false)
    {
        if (onClick != null)
        {
            if (stopPropagation)
                builder.AddEventStopPropagationAttribute(1, "onclick", true);

            builder.AddAttribute(1, "onclick", onClick);
        }

        return this;
    }

    public AttributeBuilder OnDoubleClick(EventCallback? onClick)
    {
        if (onClick != null)
            builder.AddAttribute(1, "ondblclick", onClick);

        return this;
    }

    public AttributeBuilder OnChange(EventCallback<ChangeEventArgs>? onClick)
    {
        if (onClick != null)
            builder.AddAttribute(1, "onchange", onClick);

        return this;
    }

    public AttributeBuilder OnEnter(string enter)
    {
        if (!string.IsNullOrWhiteSpace(enter))
            builder.AddAttribute(1, "onenter", enter);

        return this;
    }

    public AttributeBuilder Add(string name, bool value)
    {
        builder.AddAttribute(1, name, value);
        return this;
    }

    public AttributeBuilder Add(string name, string value)
    {
        builder.AddAttribute(1, name, value);
        return this;
    }

    public AttributeBuilder Add(string name, object value)
    {
        if (value != null)
            builder.AddAttribute(1, name, value);

        return this;
    }
}