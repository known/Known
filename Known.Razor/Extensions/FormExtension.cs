namespace Known.Razor.Extensions;

public static class FormExtension
{
    public static void FormList<T>(this RenderTreeBuilder builder, string title, int top, string style = null, Action<AttributeBuilder<T>> child = null) where T : notnull, IComponent
    {
        builder.FormList(title, top, style, () => builder.Component(child));
    }

    public static void FormList(this RenderTreeBuilder builder, string title, int top, string style = null, Action child = null)
    {
        builder.Div("form-caption", title);
        var css = CssBuilder.Default("form-list").AddClass(style).Build();
        builder.Div(css, attr =>
        {
            attr.Style($"top:{top}px;");
            child.Invoke();
        });
    }

    public static void Hidden(this RenderTreeBuilder builder, string id) => builder.Field<Hidden>(id).Build();
    public static void Hidden(this RenderTreeBuilder builder, string id, string value) => builder.Field<Hidden>(id).Value(value).Build();
    public static FieldAttrBuilder<T> Field<T>(this RenderTreeBuilder builder, string id, bool required = false) where T : Field => builder.Field<T>("", id, required);

    public static FieldAttrBuilder<T> Field<T>(this RenderTreeBuilder builder, string label, string id, bool required = false) where T : Field
    {
        var fb = new FieldAttrBuilder<T>(builder);
        fb.Set(f => f.Label, label)
          .Set(f => f.Id, id)
          .Set(f => f.Required, required);
        if (typeof(T) == typeof(Select))
            fb.Add(nameof(Select.EmptyText), "请选择");
        return fb;
    }

    public static void BuildFlowLog(this RenderTreeBuilder builder, string bizId, int colSpan)
    {
        builder.FormList<FlowLogGrid>("流程记录", colSpan, child: attr =>
        {
            attr.Set(c => c.BizId, bizId);
        });
    }
}