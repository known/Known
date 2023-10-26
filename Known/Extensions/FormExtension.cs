namespace Known.Extensions;

public static class FormExtension
{
    public static void Form(this RenderTreeBuilder builder, Action<RenderTreeBuilder> body, Action<RenderTreeBuilder> action)
    {
        builder.Div("form", attr =>
        {
            builder.Div("form-body", attr => body.Invoke(builder));
            builder.Div("form-button", attr => action.Invoke(builder));
        });
    }

    public static void FormList<T>(this RenderTreeBuilder builder, string title, string style = null, Action<AttributeBuilder<T>> child = null) where T : notnull, IComponent
    {
        builder.FormList(title, style, () => builder.Component(child));
    }

    public static void FormList(this RenderTreeBuilder builder, string title, string style = null, Action child = null)
    {
        builder.Div("form-caption", title);
        var css = CssBuilder.Default("form-list").AddClass(style).Build();
        builder.Div(css, attr => child.Invoke());
    }

    public static void Hidden(this RenderTreeBuilder builder, string id) => builder.Field<KHidden>(id).Build();
    public static void Hidden(this RenderTreeBuilder builder, string id, string value) => builder.Field<KHidden>(id).Value(value).Build();
    public static FieldAttrBuilder<T> Field<T>(this RenderTreeBuilder builder, string id, bool required = false) where T : Field => builder.Field<T>("", id, required);

    public static FieldAttrBuilder<T> Field<T>(this RenderTreeBuilder builder, string label, string id, bool required = false) where T : Field
    {
        var fb = new FieldAttrBuilder<T>(builder);
        fb.Set(f => f.Label, label)
          .Set(f => f.Id, id)
          .Set(f => f.Required, required);
        if (typeof(T) == typeof(KSelect))
            fb.Add(nameof(KSelect.EmptyText), "请选择");
        return fb;
    }

    public static void BuildFlowLog(this RenderTreeBuilder builder, string bizId)
    {
        builder.FormList<FlowLogGrid>("流程记录", "flow-log", attr =>
        {
            attr.Set(c => c.BizId, bizId);
        });
    }
}