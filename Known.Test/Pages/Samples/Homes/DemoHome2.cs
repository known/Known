namespace Known.Test.Pages.Samples.Homes;

class DemoHome2 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
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

    private void BuildBadge(RenderTreeBuilder builder, StyleType style, string text)
    {
        builder.Div("demo-badge", attr =>
        {
            builder.Text("消息中心");
            builder.Badge(style, text);
        });
    }

    private void BuildTag(RenderTreeBuilder builder, StyleType style, string text)
    {
        builder.Div("demo-tag", attr => builder.Tag(style, text));
    }

    private void BuildTag(RenderTreeBuilder builder, StyleType style, Action<RenderTreeBuilder> content)
    {
        builder.Div("demo-tag", attr => builder.Tag(style, content));
    }
}