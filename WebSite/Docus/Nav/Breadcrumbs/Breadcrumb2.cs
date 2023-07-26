namespace WebSite.Docus.Nav.Breadcrumbs;

class Breadcrumb2 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<Breadcrumb>().Set(c => c.Items, new()
        {
            new MenuItem("Test1", "测试1", "fa fa-home"),
            new MenuItem("Test2", "测试2", "fa fa-user"),
            new MenuItem("Test3", "点击试试") { Action = () => UI.Alert("点击测试...") }
        }).Build();
    }
}