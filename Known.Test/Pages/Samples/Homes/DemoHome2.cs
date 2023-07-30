namespace Known.Test.Pages.Samples.Homes;

class DemoHome2 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("demo-row", attr =>
        {
            BuildBadge(builder);
            BuildTag(builder);
            BuildProgress(builder);
        });
    }

    private static void BuildBadge(RenderTreeBuilder builder)
    {
        builder.Div("row", attr =>
        {
            BuildBadge(builder, StyleType.Default, "10");
            BuildBadge(builder, StyleType.Primary, "10");
            BuildBadge(builder, StyleType.Success, "10");
            BuildBadge(builder, StyleType.Info, "10");
            BuildBadge(builder, StyleType.Warning, "10");
            BuildBadge(builder, StyleType.Danger, "10");
        });
    }

    private static void BuildTag(RenderTreeBuilder builder)
    {
        builder.Div("row", attr =>
        {
            BuildTag(builder, StyleType.Default, "测试");
            BuildTag(builder, StyleType.Primary, "完成");
            BuildTag(builder, StyleType.Success, "通过");
            BuildTag(builder, StyleType.Info, "进行中");
            BuildTag(builder, StyleType.Warning, "警告");
            BuildTag(builder, StyleType.Danger, "失败");
            BuildTag(builder, StyleType.Success, b => b.IconName("fa fa-user", "模板"));
        });
    }

    private static void BuildProgress(RenderTreeBuilder builder)
    {
        builder.Div("row", attr =>
        {
            BuildProgress(builder, StyleType.Default, 0.5M);
            BuildProgress(builder, StyleType.Primary, 0.35M);
            BuildProgress(builder, StyleType.Success, 1);
            BuildProgress(builder, StyleType.Info, 0.6M);
            BuildProgress(builder, StyleType.Warning, 0.55M);
            BuildProgress(builder, StyleType.Danger, 0.8M);
        });
    }

    private static void BuildBadge(RenderTreeBuilder builder, StyleType style, string text)
    {
        builder.Div("demo-badge", attr =>
        {
            builder.Text("消息中心");
            builder.Badge(style, text);
        });
    }

    private static void BuildTag(RenderTreeBuilder builder, StyleType style, string text)
    {
        builder.Div("demo-tag", attr => builder.Tag(style, text));
    }

    private static void BuildTag(RenderTreeBuilder builder, StyleType style, Action<RenderTreeBuilder> content)
    {
        builder.Div("demo-tag", attr => builder.Tag(style, content));
    }

    private static void BuildProgress(RenderTreeBuilder builder, StyleType style, decimal value)
    {
        builder.Progress(style, value, 100);
    }
}