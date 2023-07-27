namespace WebSite.Docus.View.Badges;

class Badge1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        BuildBadge(builder, StyleType.Default, "10");
        BuildBadge(builder, StyleType.Primary, "10");
        BuildBadge(builder, StyleType.Success, "10");
        BuildBadge(builder, StyleType.Info, "10");
        BuildBadge(builder, StyleType.Warning, "10");
        BuildBadge(builder, StyleType.Danger, "10");
    }

    private static void BuildBadge(RenderTreeBuilder builder, StyleType style, string text)
    {
        builder.Div("demo-badge", attr =>
        {
            builder.Text("消息中心");
            builder.Badge(style, text);
        });
    }
}