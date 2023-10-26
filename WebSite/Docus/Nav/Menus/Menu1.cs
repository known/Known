namespace WebSite.Docus.Nav.Menus;

class Menu1 : BaseComponent
{
    private string? message;
    private readonly List<KMenuItem> Items = new()
    {
        new KMenuItem("Test1", "基础数据", "fa fa-database") {
            Children = new List<KMenuItem> {
                new KMenuItem("Test11", "测试11"),
                new KMenuItem("Test12", "测试12")
            }
        },
        new KMenuItem("Test2", "业务管理", "fa fa-suitcase") {
            Badge = 10,
            Children = new List<KMenuItem> {
                new KMenuItem("Test21", "测试21"),
                new KMenuItem("Test22", "测试22")
            }
        },
        new KMenuItem("Test3", "系统管理", "fa fa-cogs") { 
            Children = new List<KMenuItem> {
                new KMenuItem("Test31", "测试31"),
                new KMenuItem("Test32", "测试32")
            }
        }
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("demo-menu", attr =>
        {
            builder.Div("sider", attr =>
            {
                builder.Component<KMenu>()
                       .Set(c => c.Style, "menu-tree")
                       .Set(c => c.TextIcon, true)
                       .Set(c => c.Items, Items)
                       .Set(c => c.OnClick, OnItemClick)
                       .Build();
            });
            builder.Div("body", attr => builder.Span(message));
        });
    }

    private void OnItemClick(KMenuItem item)
    {
        message = $"点击菜单{item.Name}";
        StateChanged();
    }
}