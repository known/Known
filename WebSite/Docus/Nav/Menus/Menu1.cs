namespace WebSite.Docus.Nav.Menus;

class Menu1 : BaseComponent
{
    private string? message;
    private readonly List<MenuItem> Items = new()
    {
        new MenuItem("Test1", "基础数据", "fa fa-database") {
            Children = new List<MenuItem> {
                new MenuItem("Test11", "测试11"),
                new MenuItem("Test12", "测试12")
            }
        },
        new MenuItem("Test2", "业务管理", "fa fa-suitcase") {
            Badge = 10,
            Children = new List<MenuItem> {
                new MenuItem("Test21", "测试21"),
                new MenuItem("Test22", "测试22")
            }
        },
        new MenuItem("Test3", "系统管理", "fa fa-cogs") { 
            Children = new List<MenuItem> {
                new MenuItem("Test31", "测试31"),
                new MenuItem("Test32", "测试32")
            }
        }
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("demo-menu", attr =>
        {
            builder.Div("sider", attr =>
            {
                builder.Component<Menu>()
                       .Set(c => c.Style, "menu-tree")
                       .Set(c => c.TextIcon, true)
                       .Set(c => c.Items, Items)
                       .Set(c => c.OnClick, OnItemClick)
                       .Build();
            });
            builder.Div("body", attr => builder.Span(message));
        });
    }

    private void OnItemClick(MenuItem item)
    {
        message = $"点击菜单{item.Name}";
        StateChanged();
    }
}