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

    public void Div(string className, Action<AttributeBuilder> child = null)
    {
        Builder.Div(attr =>
        {
            attr.Class(className);
            child?.Invoke(attr);
        });
    }

    public void Table(Action<FieldBuilder<T>> child = null)
    {
        var table = new TableContext();
        Builder.Component<CascadingValue<TableContext>>(ab =>
        {
            ab.Set(c => c.IsFixed, false)
              .Set(c => c.Value, table)
              .Set(c => c.ChildContent, delegate (RenderTreeBuilder b)
              {
                  var fb = new FieldBuilder<T>(b);
                  b.Element("table", attr => child?.Invoke(fb));
              });
        });
    }

    public ComponentBuilder<TC> Component<TC>() where TC : BaseComponent => Builder.Component<TC>();
    public void Button(ButtonInfo button, EventCallback onClick, bool visible = true, string style = null) => Builder.Button(button, onClick, visible, style);
    public void Label(string className, string text) => Builder.Label(className, text);
    public void ColGroup(params int?[] widths) => Builder.ColGroup(widths);
    public void Tr(Action<AttributeBuilder> child = null) => Builder.Tr(child);
    public void Th(Action<AttributeBuilder> child = null) => Builder.Th(child);
    public void Th(string className, Action<AttributeBuilder> child = null) => Builder.Th(className, child);
    public void Th(string className, string text) => Builder.Th(className, text);
    public void Td(Action<AttributeBuilder> child = null) => Builder.Td(child);
    public void FormList<TC>(string title, int top, string style = null, Action<AttributeBuilder<TC>> child = null) where TC : notnull, IComponent => Builder.FormList(title, top, style, child);
    public void FormList(string title, int top, string style = null, Action child = null) => Builder.FormList(title, top, style, child);
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
        Set(r => r.Value, value?.ToString(Config.DateFormat)).Add(nameof(Date.Value), value);
        return this;
    }

    public FieldAttrBuilder<T> ValueChanged(Action<string> valueChanged)
    {
        Set(r => r.ValueChanged, valueChanged);
        return this;
    }

    public FieldAttrBuilder<T> InputTemplate(Action<RenderTreeBuilder> template)
    {
        Set(r => r.InputTemplate, template);
        return this;
    }
}