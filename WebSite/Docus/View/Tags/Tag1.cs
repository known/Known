namespace WebSite.Docus.View.Tags;

class Tag1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        BuildTag(builder, StyleType.Default, "测试");
        BuildTag(builder, StyleType.Primary, "完成");
        BuildTag(builder, StyleType.Success, "通过");
        BuildTag(builder, StyleType.Info, "进行中");
        BuildTag(builder, StyleType.Warning, "警告");
        BuildTag(builder, StyleType.Danger, "失败");
        BuildTag(builder, StyleType.Success, b => b.IconName("fa fa-user", "模板"));
    }

    private static void BuildTag(RenderTreeBuilder builder, StyleType style, string text)
    {
        builder.Div("demo-tag", attr => builder.Tag(style, text));
    }

    private static void BuildTag(RenderTreeBuilder builder, StyleType style, Action<RenderTreeBuilder> content)
    {
        builder.Div("demo-tag", attr => builder.Tag(style, content));
    }
}