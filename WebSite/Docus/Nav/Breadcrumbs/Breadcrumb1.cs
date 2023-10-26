namespace WebSite.Docus.Nav.Breadcrumbs;

class Breadcrumb1 : BaseComponent
{
    private readonly List<KMenuItem> Items = new()
    {
        new KMenuItem("Test1", "测试1", "fa fa-home"),
        new KMenuItem("Test2", "测试2", "fa fa-user"),
        new KMenuItem("Test3", "测试3")
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<KBreadcrumb>().Set(c => c.Items, Items).Build();
    }
}