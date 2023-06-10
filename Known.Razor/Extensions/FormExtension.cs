namespace Known.Razor.Extensions;

public static class FormExtension
{
    public static void FormCaption(this RenderTreeBuilder builder, string title, int colSpan) => builder.Tr(attr => builder.Td("form-caption", title, colSpan));

    public static void FormList<T>(this RenderTreeBuilder builder, string title, int colSpan, string style = null, int? height = null, Action<AttributeBuilder<T>> child = null) where T : notnull, IComponent
    {
        builder.FormList(title, colSpan, style, height, () => builder.Component(child));
    }

    public static void FormList(this RenderTreeBuilder builder, string title, int colSpan, string style = null, int? height = null, Action child = null)
    {
        builder.FormCaption(title, colSpan);
        builder.Tr(attr =>
        {
            builder.Td(attr =>
            {
                attr.Class(style).ColSpan(colSpan);
                if (height != null && height.HasValue)
                    attr.Style($"position:relative;height:{height}px;");
                child?.Invoke();
            });
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