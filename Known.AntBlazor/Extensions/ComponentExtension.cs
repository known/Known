namespace Known.AntBlazor.Extensions;

static class ComponentExtension
{
    internal static void Button(this RenderTreeBuilder builder, ActionInfo info)
    {
        builder.Component<Button>()
               .Set(c => c.Disabled, !info.Enabled)
               .Set(c => c.Icon, info.Icon)
               .Set(c => c.Type, info.Style)
               .Set(c => c.OnClick, info.OnClick)
               .Set(c => c.ChildContent, b => b.Text(info.Name))
               .Build();
    }

    internal static void FormItem(this RenderTreeBuilder builder, IAntField field, Action<RenderTreeBuilder> child)
    {
        builder.Component<GridCol>()
               .Set(c => c.Span, field.Span)
               .Set(c => c.ChildContent, b =>
               {
                   b.Component<FormItem>()
                    .Set(c => c.Label, field.Label)
                    .Set(c => c.Required, field.Required)
                    .Set(c => c.Rules, field.ToRules())
                    .Set(c => c.ChildContent, b1 => child.Invoke(b1))
                    .Build();
               })
               .Build();
    }
}