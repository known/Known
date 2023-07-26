namespace WebSite.Docus.Nav.Breadcrumbs;

class Breadcrumb1 : BaseComponent
{
    private readonly List<MenuItem> Items = new()
    {
        new MenuItem("Test1", "测试1", "fa fa-home"),
        new MenuItem("Test2", "测试2", "fa fa-user"),
        new MenuItem("Test3", "测试3")
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<Breadcrumb>().Set(c => c.Items, Items).Build();
    }
}