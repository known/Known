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
}