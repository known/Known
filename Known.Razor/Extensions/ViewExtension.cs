namespace Known.Razor.Extensions;

public static class ViewExtension
{
    public static void ViewLR(this RenderTreeBuilder builder, Action<AttributeBuilder> left, Action<AttributeBuilder> right)
    {
        builder.Div("lr-view", attr =>
        {
            builder.Div("left-view", left);
            builder.Div("right-view", right);
        });
    }
}