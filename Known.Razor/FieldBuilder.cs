using System.Linq.Expressions;

namespace Known.Razor;

public class FieldBuilder<T>
{
    public FieldBuilder(RenderTreeBuilder builder)
    {
        Builder = builder;
    }

    public RenderTreeBuilder Builder { get; }

    public void Hidden(Expression<Func<T, object>> selector) => Field<Hidden>(selector).Build();

    public FieldAttrBuilder<TField> Field<TField>(string label, string id, bool required = false) where TField : Field
    {
        var builder = new FieldAttrBuilder<TField>(Builder);
        builder.Set(f => f.Label, label)
               .Set(f => f.Id, id)
               .Set(f => f.Required, required);
        if (typeof(TField) == typeof(Select))
            builder.Add(nameof(Select.EmptyText), "请选择");
        return builder;
    }

    public FieldAttrBuilder<TField> Field<TField>(Expression<Func<T, object>> selector) where TField : Field
    {
        var propertyInfo = TypeHelper.Property(selector);
        var paramAttr = propertyInfo.GetCustomAttribute<ColumnAttribute>(true);
        var required = paramAttr != null && paramAttr.Required;
        return Field<TField>(paramAttr?.Description, propertyInfo.Name, required);
    }

    public RenderTreeBuilder Div(string className, Action<AttributeBuilder> child = null)
    {
        return Builder.Div(attr =>
        {
            attr.Class(className);
            child?.Invoke(attr);
        });
    }

    public RenderTreeBuilder Table(Action<RenderTreeBuilder> child = null) => Builder.Table(attr => child?.Invoke(Builder));
    public RenderTreeBuilder Table(string className, Action<RenderTreeBuilder> child = null) => Builder.Table(className, attr => child?.Invoke(Builder));
}

public class FieldAttrBuilder<T> : ComponentBuilder<T> where T : Field
{
    internal FieldAttrBuilder(RenderTreeBuilder builder) : base(builder) { }

    public FieldAttrBuilder<T> Label(string label)
    {
        Set(r => r.Label, label);
        return this;
    }

    public FieldAttrBuilder<T> Required(bool required)
    {
        Set(r => r.Required, required);
        return this;
    }

    public FieldAttrBuilder<T> IsInput(bool isInput)
    {
        Set(r => r.IsInput, isInput);
        return this;
    }

    public FieldAttrBuilder<T> ColSpan(int colSpan)
    {
        Set(r => r.ColSpan, colSpan);
        return this;
    }

    public FieldAttrBuilder<T> RowSpan(int rowSpan)
    {
        Set(r => r.RowSpan, rowSpan);
        return this;
    }

    public FieldAttrBuilder<T> Style(string style)
    {
        Set(r => r.Style, style);
        return this;
    }

    public FieldAttrBuilder<T> Value(string value)
    {
        Set(r => r.Value, value);
        return this;
    }

    public FieldAttrBuilder<T> Value(DateTime? value)
    {
        Set(r => r.Value, value?.ToString(Config.DateFormat)).Add(nameof(Date.Day), value);
        return this;
    }

    public FieldAttrBuilder<T> ValueChanged(Action<string> valueChanged)
    {
        Set(r => r.ValueChanged, valueChanged);
        return this;
    }
}