namespace WebSite.Docus.Nav.Menus;

class Menu2 : BaseComponent
{
    private string? message;
    private readonly List<MenuItem> Items = new()
    {
        new MenuItem("Test1", "基础数据", "fa fa-database"),
        new MenuItem("Test2", "业务管理", "fa fa-suitcase"),
        new MenuItem("Test3", "系统管理", "fa fa-cogs")
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("demo-menu", attr =>
        {
            builder.Div("sider", attr =>
            {
                builder.Component<Menu>()
                       .Set(c => c.Style, "menu-tab")
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